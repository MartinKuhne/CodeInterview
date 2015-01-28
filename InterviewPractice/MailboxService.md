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

I will specifiy additional requirements after answering Question 2

## Question 2

Design goals (some paraphrased from question)
- As an administrator, I can move mailboxes between machines with no service interruption
- As a customer, the contents of my mailbox are always available
- As a customer, mail delivery is instant 99% of the time and can take up to a minute 1% of the time 

Now we can test different solutions against the requirements

First off, there's a solution in Windows Hyper-V called live migration where you can literally move a running VM to a different machine. It requires a SAN obviously. You can build a SAN out of commodity hardware (like backblaze). Even if you don't have live migration, the mailbox state must live on disk, so if you have a SAN, storage can fail over really quickly and the agent can restart. For the purpose of this design, I am going to assume everything lives on the commodity machine and we don't have a SAN

We don't have SQL and we don't need all of ACID. The problem we have to solve though is that of forward progress, in other words once a new e-mail has arrived, if we revert to a previous state, the user will experience data loss. The same will apply to message deletion or locally created messages.

I will assume there is a persistent indexed storage which I will call mailbox and a user agent controlling the mailbox which I will call user agent. Furthermore there is a mapping from user to the machine the mailbox and user agent runs on which I will call mapping.

### Option 1
If we introduce a mailbox read-only state, we can go through a series of state changes to preserve forward progress:
- redirect incoming messages to a queue
- switch mailbox to read-only
- migrate mailbox to new machine
- update the mapping
- stop the old user agent (force client to reconnect)
- restart message delivery to new agent
- delete the old mailbox

This solution is fairly straightforward to implement. It meets the design goal of migration with no data loss. However, message deletes will be unavailable while the mailbox is read-only and the duration of the read-only state will increase linearly with the size of the mailbox.

### Option 2 (reconcile)
- snapshot and migrate mailbox to new machine (new messages arrive in old mailbox)
- update the mapping (new messages arrive in new mailbox)
- stop the old user agent (force client to reconnect)
- restart message delivery to new agent
- reconcile

This approach has us maintain the mailbox state in two locations. We would rely on strong message identity (e.g. SMTP message id) to make the mailbox "eventually consistent".

### Option 3 (copy-on-write)
This is a variant of #2 where we snapshot the mailbox, then keep a record of any changes that happen after the snapshot. This can be done with VSS in Windows and BTRFS snaphots on linux. This makes the reconcile operation fairly foolproof.

### Option 4 (null hypothesis)
Do nothing - mailbox down during migration

The reason this will be a solution considered is that's it's zero cost and known risk. Sometimes that's the best option. 
