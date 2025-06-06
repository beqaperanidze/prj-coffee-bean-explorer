### What Was Changed

- Added Create and Update Mapping Objects. This enables direct mapping from DTOs to entities for create and update
  operations.


- Updated Service Usage:
  The Service classes now uses mapper creation updates, removing manual property assignments.

### Why These Changes Were Made

- Simplified Service Logic:
  Service methods are now cleaner, focusing on business logic rather than object construction.


- Centralized Mapping Rules:
  All mapping logic for create and update operations is now in the profile, making it easier to maintain and update.


- Consistency:
  Ensures that mapping behavior is uniform and less error-prone across the application.
