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
            var parsedPosition = new Position(position);
            var rover = new MarsRover(_world, parsedPosition);
            rovers.Add(rover);
        }

        [When(@"I issue the command sequence '(.*)' to Rover '(.*)'")]
        public void ExecuteCommand(string command, string unit)
        {
            var rovers = _scenarioContext.Get<List<MarsRover>>(KeyMarsRover);
            var rover = rovers[int.Parse(unit)];
            rover.Move(command);
        }

        [Then(@"the new position is '(.*)' for Rover '(.*)'")]
        public void TestPosition(string destination, string unit)
        {
            var rovers = _scenarioContext.Get<List<MarsRover>>(KeyMarsRover);
            var rover = rovers[int.Parse(unit)];

            var parsedPosition = new Position(destination);
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
