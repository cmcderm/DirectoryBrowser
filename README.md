# Directory Browser API + Frontend
### Written by Connor McDermott

This project is written on Linux and expects to use Linux filepaths.
I used .NET 8 Core and nvim, with a TS frontend, no UI libraries or frameworks.
Testing was done with postman.

## Running the project

1. Clone the repository.
1. Navigate to the project directory.
1. Run `npm install && tsc` to install dependencies and transpile the typescript.
1. Run `dotnet run` to start the backend server.

## Overview

The primary endpoint is at /api/path/<path>, allowing for easy browsing of directories.
The frontend also allows for simple browsing of directories and files, where the URL path is localhost:3000/app<path>.
