# NCLcut: Efficient NCL File Shortening Tool

## Overview
NCLcut is a specialized tool designed to efficiently shorten large NCL (Numerical Control Language) files used in post-processor runs. Developed for scenarios where extensive NCL files lead to prolonged testing times, NCLcut allows users to selectively trim these files, focusing on essential parts for testing.

## Features
- **Open File**: Load NCL files (.ncl) through a user-friendly dialog window.
- **File Information Display**: Shows the filename, path, and file size.
- **Sequence Display**: Lists sequences with details like running number, sequence name, feature number, line count, and tool presence.
- **Selective Shortening**: Choose individual or multiple sequences for trimming.
- **Customizable GOTO Limit**: Set a limit for the number of GOTO movements (default: 30).
- **Cutting and Deleting Options**: Trim or remove selected sequences.
- **File Size Update**: View the new file size post-modification.
- **Save Options**: Overwrite the existing file or save as a new file.
- **Sequence Preview**: Non-editable window to view the start of the selected sequence.

## Functionality
- **Line-by-Line Reading**: NCL files are read into memory one line at a time.
- **Sequence Detection**: Identifies sequence start (`->FEATNO/`), end (`->END/`), name (`PPRINT / NC SEQUENCE NAME:`), tools (`LOADTL` or `TURRET`), circles (`CIRCLE`), GOTOs, continuation lines (`$`), and file end (`FINI`).
- **Object Creation**: Each sequence's data is stored in an object, with planned changes saved initially in the object.
- **Continuation Lines**: Ensures 1:1 output of continuation lines as in the input file.
- **Circle Handling**: Prevents separation of GOTOs following a CIRCLE command.
- **End-of-Sequence Handling**: Cuts only GOTOs at the end of a sequence, keeping technology instructions intact.

## Requirements
- **Programming Environment**: C++, Visual Studio 2019, preferably using MFC.
- **Platform**: Windows executable, standalone.

## Usage
1. **Open File**: Click 'Open File' to load an NCL file.
2. **Select Sequences**: Choose sequences to shorten or delete.
3. **Set GOTO Limit**: Adjust the number of remaining GOTOs if needed.
4. **Modify File**: Use 'Cutting' or 'Deleting' options as required.
5. **Save Changes**: Click 'Save' or 'Save as' to finalize changes.

## Note
This tool is designed to streamline the process of shortening NCL files for testing purposes, ensuring efficiency and accuracy in handling large files.

---

For more information, updates, or to contribute, please visit our [GitHub repository](https://github.com/your-repository-link).
