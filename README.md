**Functionality**
1. Real-Time Elevator Status
Display the real-time status of each elevator, including its current floor, direction of movement,
whether it's in motion or stationary, and the number of passengers it is carrying.
2. Interactive Elevator Control
Due to nature of the console app, this is not possible  in real time, and can only be achieved before starting the elevatrors.
However it can be achivbed in the web application example 
3. Multiple Floors and Elevators Support
Support for any number of floors as well as basement levels
4. Efficient Elevator Dispatching
Dispathex the closest elevator dependant apon movement
5. Passenger Limit Handling
When pasangers limit is reached, the elevator will move, leaving the remaning passangers for the next elevator
6. Consideration for Different Elevator Types
3 elevators types defines, but code can be enhanced to add more.
7. Real-Time Operation
No in the console app.

**How are the elevators controled?**
Pickup loads are stored in a central list to be quered by the elevators
The central process gets the closest elevator to a particular floor for load pickup.
The central process will instruct the elevaltor to move to that floor.
The movement of the elevator is handeld by the elevator itself.
The elevator will check each floor it passes to check for any loads to be picked up and pick up a any load going in the same direction as the elevator movement.
The elevator will contain a list of loads to be droped off at differnet floors and will stop at each of those floors drop off and load any waiting loads.
Since the elevators move independantly, it is possible for elevators to compete with each other in load pickups and when a load is picked up, other elevators will not be able to pick up the load.
Elevators will stop moving when no loads are avalable.


