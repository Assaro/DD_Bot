# **Command Overview**

### /admin
- choose a user to grant or revoke admin privilieges to
- Can only be done by admins
- An admin can not revoke his own admin privileges

### /docker
- used to start, stop or restart a docker container
- admins are allowed to control every container
- users can only start and stop containers that have been assigned to them by admins
- start and stop permissions are granted separately
- permissions can be done per user and for roles

### /list
- this command lists all containers the user is allowed to interact with 
  - includes the status (running or stopped)
- admins see every container

### /permission
- lists a users permissions
- admins can enter a user or role for permissions to be shown

### /role
- used to grant/revoke permissions to a role

### /role
- used to grant/ revoke permissions to a single user