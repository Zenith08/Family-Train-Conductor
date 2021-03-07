# Family Train Conductor Design Mockup

Family train conductor is a couch co-op train driving game taking inspiration from Trainz: A New Era and Overcooked.

![Full View](/docs/game-overview-ui.jpg)
![Phone Driving](/docs/phone-mockup.jpg)
![Phone Selection](/docs/phone-mode-select.jpg)

## Game Flow: Startup
* All players connect phones/laptops as controllers
* They choose and enter a map/level

## Game Flow: In Level
* Load level
* Players can claim a train
* Players can drive train around map switching points as necessary
* The goal of the player is to keep industries running while serving passenger runs
* Players communicate with teammates to maximize efficiency

## Train Control
* Players select a train to control from their phones at the start of the level. The switch control arrows control the next branching switch in front of the train
* The train has control over the switch directly in front of it but not the next one along

![Switch Example](/docs/switch-explanation-1.jpg)

* This train does not need control over the switch and will cross the switch automatically

![Switch Example 2](/docs/switch-explanation-2.jpg)

* If the train is reversed, then switch control will be the switch behind the train
* If the train is reversed (Maybe pushing?) The train will be speed limited to encourage pulling carriages. Some trains will be bi-directional like subways

## Industries

![Industries](/docs/industry-picture.jpg)

* Industries are characterized by a building and an associated track area.
* Industries are one of:
	* Creators - produces resources over time
	* Convertors - Converts one resource into another over time
	* Consumers - Consumes resources over time
* If a converting industry runs out of consumption resources, it stops running
* If a consuming industry runs out of resources the level ends (Game Over)
* When a train is on an industries tracks and the rolling stock is compatible with resources, the rolling stock will be loaded
* Loaded rolling stock will be unloaded if the industry is compatible
* These are transfers of resources to and from industries as they are produced, converted, or consumed

## Passenger Services
* Passenger services are scheduled, this schedule is visible to the player via a top bar of widgets
* The schedule defines a route for a train to take and a countdown until the train must leave

![Passenger Services 1](/docs/passenger-widget-1.jpg)

* When the countdown hits 00:00 the first passenger train at the start of the route is assigned the service
* When the countdown reaches 00:00 it starts counting up, the assigned train must complete the route as quickly as possible. Completing the route faster results in a boost to production


![Passenger Services 2](/docs/passenger-widget-2.jpg)

## Proposed Story + Levels
The story is very basic and is simply that the group of players is going to work as conductors on a rail network. Their job will be to take control of areas of tracks and keep the local industries and passenger runs running.

### Level 1 - Basic Introduction
* Introduce basic controls
* Objective is to complete one circle around forwards and one circle backwards
* (Yes it’s supposed to be circles)

![Level 1](/docs/levels/first-level.jpg)

### Level 2 - Switch Control
* Introduces switch controls
* Forces communication
* Goal is to make the trains on the right swap with trains on the left

![Level 2](/docs/levels/level-2.jpg)

### Level 3 - Industries Introduction
* Introduces industries
* Increases communications requirement
* Should be fairly easy
* Industries are:
	* Top right: Producer (ie. Forest)
	* Far left: converter (ie. Wood Chipper)
	* Bottom: Consumer (ie. Paper Mill)

![Level 3](/docs/levels/level-3.jpg)

### Level 4 - Introduces Passenger Services
* Introduces passenger services
* Bi-directional trains
* Regularly scheduled subway service promotes a cycle
* Single line area promotes communication

![Level 3](/docs/levels/level-4.jpg)

### Level 5 - Full Gameplay
* Introduces simultaneous passenger and freight service
* Industries are: Top brick building, middle lumber yard, bottom industrial building
* Stations are: Top, left hand side (double), bottom (double)
* Passenger trains are bi-directional OR options for train run-arounds / turntables are made available

![Level 3](/docs/levels/level-5-updated.jpg)


## Unused Ideas
### Shunters vs. Haulers
* Hauler engines are big and faster, they can decouple from their carriages but not specific carriages
![Hauler](/docs/hauler-demo-labeled.jpg)
* Shunters are small locomotives that are slower but can split specific carriages
![Shunter](/docs/shunter-demo-labeled.jpg)
* Allows players to customize trains, but makes the game more complex

### Level Themes
* Amtrak
* Summer
* Winter
* Metro Line
* Red Car Trolley
* Tram with cars as obstacles
* Coal mining town
* Mineshaft controlling minecarts
* Train Ferry Terminal
* Snow Plow Based Level
* Carnival Railroad (summer)
* Romantic Moonlight Passenger Run (Valentine’s Day)
* Christmas (Polar Express or similar
* Thomas the Tank Engine (Licensed DLC)

### Miscellaneous
* Bridges and Tunnels
* Portals
	* A train can enter a portal and leave the game environment
	* Trains can spawn from a portal and either travel a predetermined path or require claiming and driving by the players
* Turntables
* Steam Engine Fuel Requirements
