# Control Program
Control program is made to control an antenna rotator based on the EASYCOMIII protocol. Control Program only looks at celestrial objects.

## What the program needs to do:
* Calculate the AZ and EL of planets from most recent JPL Horizons data and old

* Communicate with antenna rotor controller (serial)
    * Read from (location & stats) and send to device

* Display all information on VT4100 terminal
    * Current location of antenna
    * Location off all major bodies above horizon and current location of antenna on a mapped display
    * All calculated values of followed objects
    * A way to enter commands, e.g. "follow mars"
    * display of data from remote antenna controller.


## Classes:

* Comms
* Calculator
* Terminal
    * Window manager
* Remote
* logger class
* main?

a way to communicate between all classes nicely?

## Notes:

??