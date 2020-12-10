# Mars Rovers

Mars Rovers was presented to me as an interview challenge. I think it's an interesting vehicle to demonstrate skills and experience,
albeit asking quite a few trade-offs of the candidate keeping available time in mind.

The question text isn't mine to share. I found the question repeated at [Tech interview puzzles](http://www.techinterviewpuzzles.com/2010/09/mars-rovers-thoughtworks-puzzles.html). The short version is to procure a framework to move one or more objects in a two-dimensional bounded space using a series of commands. These commands are turn left or right, or move forward one coordinate unit.

> This is a C#/.Net 5.0 project. It requires the free .NET 5.0 SDK to run. My editor is Visual Studio Code.

## Project layout

Folder                | Description                                  | Run command 
--------------------  | -------------------------------------------  | ----------- 
RoverLib              | Business logic                               | n/a
Rover                 | Commandline program to demonstrate the code  | dotnet run
RoverTest             | Unit tests                                   | dotnet test
RoverIntegrationTest  | Integration tests                            | dotnet test

## Design

Some of the *interesting* design decisions were

* Who moves the unit(s)
* Correct behaviour for a collision or going out of bounds
* Naturally, how fancy to make the code vs. time constraints

Two models were considered to decide who should move the units

1. A supervisor mode, where a central controller issues or proxies the move commands
2. A collaborative mode, where units move on their own and use a controller service to update their location

The second approach was chosen, to account for the possible nature of a Mars expedition, favoring greater unit independence over tighter control.

In this model, the CanMoveTo() and ReportLocation() functions are part of the navigation interface, which is injected for the MarsRover class (see INavigation.cs)

Given the commands to the units are issued in turns and moves, not relative or absolute coordinates, any failed move step would require the unit to stop since following the rest of the move plan
would direct the unit to a different destination. Retries or path corrections were not required as per spec (but would be fun to implement!)

## Recommended reading order

* RoverLib\INavigation.cs
* RoverLib\MarsRover.cs and RoverLib\World.cs
* RoverIntegrationTest\Navigation.feature and Navigation.Steps.cs

## Showing off

Here are some cool design patterns and implementation specifics worth mentioning:
* Use of BDD in form of SpecFlow driven integration tests. This provides useful tests as well as excellent documentation of the expected behaviour of the system. Also, these tests are potentially a better use of developer time when there are time constraints over unit tests, as they test more code.
* Use of data sets to improve productivity. Two functions in CommandParserTests.cs generate a total of 10 tests. Using fluent assertions for a property wise object compare avoid writing extensive boilerplate code.

## Ambiguous test case

The *Test Input 2* test case is ambiguous. The second vehicle would follow this path

~~~
start 1 2 W
L: 1 2 S
M: 1 1 S
L: 1 1 E
M: 2 1 E 
R: 2 1 S
M: 2 0 S
~~~

However, the first rover ended at 2 1, so the second rover would pass through the same square. In my opinion, the second rover would stop at 1 1.

## Notes

* Concurrency was not requested, but I left it in as it was implemented already
* There is always more to do, dependency injection, making the code more testable by removing direct calls to Console.* etc.

> Disclaimer: I created this code entirely on my own
