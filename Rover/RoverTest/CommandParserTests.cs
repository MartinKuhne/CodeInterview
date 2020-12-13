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
    public class SimulationRequestValidatorTests
    {
        [Test]
        [TestCase(null, typeof(ArgumentException))]
        [TestCase("", typeof(ArgumentException))]
        [TestCase(" " , typeof(ArgumentException))]
        [TestCase("1 2 3 4" , typeof(ArgumentException))]
        [TestCase("-1 -1 N" , typeof(ArgumentException))]
        [TestCase("0 0 A" , typeof(ArgumentException))]
        public void SimulationRequestValidator_WhenInputIsInvalid_ThrowsException(string input, Type expectedException)
        {
            // Arrange
            Type actualResult = null;
            
            // Act
            try {
                SimulationRequestValidator.ValidatePosition(input);
            }
            catch (Exception exception)
            {
                actualResult = exception.GetType();
            }

            // Assert
            Assert.AreEqual(expectedException, actualResult);
        }
    }
}
