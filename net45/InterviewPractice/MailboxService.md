# Problem

Consider a large-scale email system such as hotmail. When a user logs in to hotmail, the user is re-directed to a particular server (ex: https://server18346.hotmail.com) where the user’s mailbox is located. A mailbox server can only accommodate so many mailboxes, though. After a while, the operations team will need to add more servers, and move mailboxes from one server to another (ex: to balance things).

## Question 1

Assume you can’t buy expensive hardware (no SQL database with many GBs of memory, algorithms are cheaper), how would you implement the hotmail locator service so that:

- Finding the right mailbox server is fast
- The locator service can easily accommodate mailboxes being moved from one server to another. 

## Question 2

What would be the procedure for moving one mailbox from one server to another server without disrupting user log ins / email delivery (both of which query the locator service)?

# Solution discussion

## Question 1

The referrer itself is a service that needs to support CRUD for key/value pairs of `UserName,Host`. The moving parts are a REST API and serialization to disk to persist the data.
Lookups are much more frequent than updates. The per-user data is of limited size, so an in-memory hash table would be convenient. 
If the data does not fit in memory, have a two-stage lookup where the first stage redirects all users starting with 'a' to a certain server, or use another hash for that.

The referrer serves two kinds of queries (redirect user to mailbox and redirect MTA to mailbox). Some approaches require these two requests to go to different hosts.

## Question 2

Design goals (some paraphrased from question)
- As an administrator, I can move mailboxes between machines with no service interruption
- As a customer, the contents of my mailbox are always available
- As a customer, mail delivery is instant 99% of the time and can take up to a minute 1% of the time

Now we can test different solutions against the requirements

### Off-the-shelf options

#### Live migration
Windows Hyper-V has the capability of live migration where one can literally move a running VM to a different machine. 
I think it requires a SAN so no storage is being physically moved.

Analysis: A SAN cen be made out of commodity hardware (like backblaze). However, running each user agent in its own VM will be prohibitively resource intensive. One could develop a design where a user agent is first migrated to its own VM, then switches hosts.

#### Distributed file systems
In a distributed file system, data is split into chunks and the chunks can live on multiple machines. This allows rebalaning on the block storage level, but writes can be are complex. This is probably exceeding the scope of an interview question :)

### Analysis

The core problem to solve is how to maintain consistency and prevent data loss during the transition to a new host. 
I will assume there is a persisted storage componentwhich I will call mailbox and a user agent controlling the mailbox which I will call user agent. 
Furthermore there is a mapping from user to the machine the mailbox and user agent runs on which I will call mapping.

I'll assume the user agent can be restarted at will (all the stata will be in the mailbox) and the client has a retry and reconnect logic.

State changes to the mailbox can occur both from the MTA (i.e. new incoming message) and the client (new outgoing message, message deletion). 

### Option 1 (null hypothesis)
Do nothing - mailbox down during migration

The reason this will be a solution considered is that's it's zero cost and known risk. Sometimes that's the best option. 

### Option 2 (Optimisitc execution)
- Clear the dirty bit on the source
- Start migrating mailbox from source to target
- Any updates to the source, set the dirty bit
- If the migration finishes and the dirty bit is not set, success
- If the dirty bit is set, retry

This is a low cost solution that can work well when run at night hours or in a maintenance window. 
However, as it is not deterministic, it would need to be combined with at least one other approach to fall back to (which may be #1)

### Option 2 (reconcile)

- migrate mailbox to to target while source remains online (new messages arrive in source)
- update the mapping (new messages arrive in new mailbox)
- stop the old user agent (force client to reconnect)
- reconcile by comparing all objects in target to all objects in source

This approach has us temporarily maintain the mailbox state in two locations. 
We would rely on strong message identity (e.g. SMTP message id) to make the mailbox "eventually consistent" with no data loss.

The reconcile step will be time and resource intensive. The user may temporarily see an older mailbox state while the reconcile is in progress.

### Option 3 (replay)

- start a log of all operations changing the mailbox state
- migrate mailbox to to target while source remains online (new messages arrive in source)
- update the mapping (new messages arrive in new mailbox)
- stop the old user agent (force client to reconnect)
- reconcile by replaying the operations onto the target

This approach is a faster variant of the previous. The reconcile step will be less time and resource intensive. 
The user may briefly see an older mailbox state while the reconcile is in progress.

### Option 3 (dual storage user agent)

This approach requires the user agent to access two mailboxes simultaneously while providing a unified view to the client.
Using this capability, we can create an empty mailbox at the target and move content in small batches, keeping user lockout 
at a minimum. Variants of this approach would be for the user agent to buffer new messages in memory, or create an additional mailbox
at another location for new messages exclusively

### Option 4 (copy-on-write)

This option pushes the problem of snapshot and delta management to the file or block level. 
A snapshot is a consistent read-only copy of a data set that allows repeatable reads. This allows mailbox migration over time.
While the migration is going on, any changes are written to another storage location. That technique is called copy-on-write (COW).
COW is available in software with VSS in Windows and BTRFS snaphots on linux.

### Option 5 (domain specific)

Depending on the protocol used, we may be able to remporarily reject incoming new messages while asking the sender to retry.

# Conclusion

I think we don't quite have enough information to pick a best solution. The next step would be to explore the feasability and maturity of the approaches discussed,
as well as an assessment of the user expectations towards consistency. For example, if the user was OK with a previously deleted message briefly reappearing,
we can in turn tune for more performance or lower cost.
