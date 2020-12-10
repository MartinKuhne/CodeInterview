using FluentAssertions;
using NUnit.Framework;
using Rover.Lib;
using System;

namespace Rover.Test
{
    /// <summary>
    /// Demonstrate use of nunit, test case data, fluent assertions, arrange/act/assert pattern.
    /// As this is a showcase, test coverage is not 100%.
    /// </summary>
    [TestFixture]
    public class CommandParserTests
    {
        [Test]
        [TestCase(null, typeof(ArgumentException))]
        [TestCase("", typeof(ArgumentException))]
        [TestCase(" " , typeof(ArgumentException))]
        [TestCase("1 2 3 4" , typeof(ArgumentException))]
        [TestCase("-1 -1 N" , typeof(ArgumentException))]
        [TestCase("0 0 A" , typeof(ArgumentException))]
        public void CommandParser_WhenInputIsInvalid_ThrowsException(string input, Type expectedException)
        {
            // Arrange
            Type actualResult = null;
            
            // Act
            try {
                CommandParser.ParsePosition(input);
            }
            catch (Exception exception)
            {
                actualResult = exception.GetType();
            }

            // Assert
            Assert.AreEqual(expectedException, actualResult);
        }

        [Test]
        [TestCase("0 0 N" , CardinalDirection.N)]
        [TestCase("0 0 E" , CardinalDirection.E)]
        [TestCase("0 0 S" , CardinalDirection.S)]
        [TestCase("0 0 W" , CardinalDirection.W)]
        public void CommandParser_WhenInputIsValid_ReturnsPosition(string input, CardinalDirection direction)
        {
            // Arrange
            var expected = new Position {
                Direction = direction,
                Point = new Point(0, 0)
            };

            // Act
            var actual = CommandParser.ParsePosition(input);

            // Assert
            // Use fluent assertions for property-wise object comparison
            expected.Should().BeEquivalentTo(actual);
        }
    }
}
