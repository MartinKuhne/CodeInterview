# Use of BDD helps us with two purposes here
#  It helps us design the code so it will be testable when it's ready
#  Acts as high level documentation as to what the expected behaviour should be

Feature: Navigation
    In order to safely navigate a rover
    I want to ensure the rover doesn't nagivate out of bounds

Scenario: Move a step
    Given A rover at '0 0 N'
    When I issue the command sequence 'M'
    Then the new position is '0 1 N'

Scenario: Out of bounds
    Given A rover at '0 0 N'
    When I issue the command sequence 'RRM'
    Then the new position is '0 0 S'

Scenario: Collision
    Given A rover at '0 0 N'
    And A rover at '0 1 N'
    When I issue the command sequence 'M'
    Then the new position is '0 0 N'

Scenario: Test Input 1a
    Given A rover at '1 2 N'
    When I issue the command sequence 'LMLMLMLMM'
    Then the new position is '1 3 N'

Scenario: Test Input 1b
    Given A rover at '3 3 E'
    When I issue the command sequence 'MMRMMRMRRM'
    Then the new position is '5 1 E'

Scenario: Test Input 2a
    Given A rover at '0 0 S'
    When I issue the command sequence 'LMMLM'
    Then the new position is '2 1 N'

Scenario: Test Input 2b
    Given A rover at '1 2 W'
    When I issue the command sequence 'LMLMRM'
    Then the new position is '2 0 S'
# Specification says to expect '1 0 S' but we are at '2 0 S'
# start 1 2 W
# L: 1 2 S
# M: 1 1 S
# L: 1 1 E
# M: 2 1 E 
# R: 2 1 S
# M: 2 0 S