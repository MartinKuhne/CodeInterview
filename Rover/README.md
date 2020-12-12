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

## Architecture decision log

An architecture decision log documents decisions made during development and more importantly, whys of the decisions and other options considered. This can be very helpful down the road, when these decisions need to be understood and sometimes changed.

* Who moves the unit(s)
* Correct behaviour for a collision or going out of bounds

Two models were considered to decide who should move the units

1. A supervisor mode, where a central controller issues or proxies the move commands
2. A collaborative mode, where units move on their own and use a controller service to update their location

The second approach was chosen, to account for the possible nature of a Mars expedition, favoring greater unit independence over tighter control. In this model, the CanMoveTo() and ReportLocation() functions are part of the navigation interface, which is injected for the MarsRover class (see INavigation.cs).

This implementation stops moving the rover when it encounters a collision or runs out of bounds. This runs counter to the expectation in test case two, which seems to expect us to ignore just the failed command and continue. The reason for deviating from the test case is as follows: In a series of moves, the output of the previous step (the updated coordinates) becomes the input of the following step. If we modify the input, the rover will follow a different path and end up in a different location. It is simply unknown if issuer of the command intended that or not. If we cannot execute the command as indicated, we should not execute it. Or, to put it bluntly, do you want to explain to congress why their $400m rover veered off course and fell down a cliff?

In fact, a more correct solution is to remain at the starting point if the command cannot be executed fully.

> No doubt the question was designed to be ambiguous and test the candidate's dealing with the same. In the real world, a scrum team would not allow such a story to enter the development stage, and seek clarification from the product owner.

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

However, the first rover ended at 2 1, so the second rover would pass through the same square.

## Future improvements

- [ ] Do not leave the starting point if the rover would collide or run out of bounds
- [ ] Refactor the command line interface to not be tightly coupled to Console.WriteLine (code as is is not testable)
- [ ] Remove the concurrency and reservation system as it was not required by the spec
- [ ] Reject initial input if the rovers start out of bounds

> Disclaimer: I created this code entirely on my own
