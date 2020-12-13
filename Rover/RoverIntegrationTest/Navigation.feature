# Use of BDD helps us with two purposes here
#  It helps us design the code so it will be testable when it's ready
#  Acts as high level documentation as to what the expected behaviour should be

Feature: Navigation
    In order to safely navigate a rover
    I want to ensure the rover doesn't nagivate out of bounds

Scenario: Move a step
    Given A rover at '0 0 N'
    When I issue the command sequence 'M' to Rover '0'
    Then the new position is '0 1 N' for Rover '0'

Scenario: Out of bounds
    Given A rover at '0 0 N'
    When I issue the command sequence 'RRM' to Rover '0'
    Then the new position is '0 0 S' for Rover '0'

Scenario: Collision
    Given A rover at '0 0 N'
    And A rover at '0 1 N'
    When I issue the command sequence 'M' to Rover '0'
    Then the new position is '0 0 N' for Rover '0'

Scenario: Test Input 1a
    Given A rover at '1 2 N'
    When I issue the command sequence 'LMLMLMLMM' to Rover '0'
    Then the new position is '1 3 N' for Rover '0'

Scenario: Test Input 1b
    Given A rover at '3 3 E'
    When I issue the command sequence 'MMRMMRMRRM' to Rover '0'
    Then the new position is '5 1 E' for Rover '0'

Scenario: Test Input 2a
    Given A rover at '0 0 S'
    When I issue the command sequence 'LMMLM' to Rover '0'
    Then the new position is '2 1 N' for Rover '0'

Scenario: Test Input 2b
    Given A rover at '0 0 S'
    And A rover at '1 2 W'
    When I issue the command sequence 'LMMLM' to Rover '0'
    And I issue the command sequence 'LMLMRM' to Rover '1'
    Then the new position is '2 1 N' for Rover '0'
 # other rover is blocking '2 1' 
    And the new position is '1 1 E' for Rover '1'
