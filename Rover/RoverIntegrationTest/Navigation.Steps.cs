using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Rover.Lib;
using TechTalk.SpecFlow;

namespace RoverIntegrationTest
{
    [Binding]
    public class NavigationSteps
    {
        private static readonly Point MapSize = new Point(100, 100);

        private readonly ScenarioContext _scenarioContext;

        private readonly World _world;

        private const string KeyMarsRover = "MarsRover";

        public NavigationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _world = new World(MapSize);
            _scenarioContext.Add(KeyMarsRover, new List<MarsRover>());
        }

        [Given(@"A rover at '(.*)'")]
        public void AddRover(string position)
        {
            var rovers = _scenarioContext.Get<List<MarsRover>>(KeyMarsRover);
            var parsedPosition = CommandParser.ParsePosition(position);
            var rover = new MarsRover(_world, parsedPosition);
            rovers.Add(rover);
        }

        [When(@"I issue the command sequence '(.*)'")]
        public void ExecuteCommand(string command)
        {
            var rovers = _scenarioContext.Get<List<MarsRover>>(KeyMarsRover);
            // Constraint: Can only issue commands to first rover
            var rover = rovers.First();
            rover.Move(command);
        }

        [Then(@"the new position is '(.*)'")]
        public void TestPosition(string destination)
        {
            var rovers = _scenarioContext.Get<List<MarsRover>>(KeyMarsRover);
            var rover = rovers.First();

            var parsedPosition = CommandParser.ParsePosition(destination);
            rover.Position.Should().BeEquivalentTo(parsedPosition);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            var rovers = _scenarioContext.Get<List<MarsRover>>(KeyMarsRover);
            rovers.ForEach(r => r.Dispose());
            _scenarioContext.Remove(KeyMarsRover);
        }
    }
}
