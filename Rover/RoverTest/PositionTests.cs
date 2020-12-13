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
    public class PositionTests
    {
        [Test]
        [TestCase("0 0 N" , CardinalDirection.N)]
        [TestCase("0 0 E" , CardinalDirection.E)]
        [TestCase("0 0 S" , CardinalDirection.S)]
        [TestCase("0 0 W" , CardinalDirection.W)]
        public void Position_WhenInputIsValid_ReturnsPosition(string input, CardinalDirection direction)
        {
            // Arrange
            var expected = new Position {
                Direction = direction,
                Point = new Point(0, 0)
            };

            // Act
            var actual = new Position(input);

            // Assert
            // Use fluent assertions for property-wise object comparison
            expected.Should().BeEquivalentTo(actual);
        }
    }
}
