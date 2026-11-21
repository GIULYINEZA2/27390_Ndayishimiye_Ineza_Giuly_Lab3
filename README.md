# Lab Test Request and Results Management System

A Windows desktop application that digitises a medical laboratory's workflow: examinees request tests, technicians collect and process specimens, and examinees later read their released results.

**Student:** Ndayishimiye Ineza Giuly (ID 27390)
**Course:** .NET, Lab 3
**Stack:** C# WinForms (.NET), MVC, MariaDB (XAMPP), MySqlConnector, ADO.NET

## Features
- **Examinee management:** add, update, delete, search and list examinees.
- **Test requests:** an examinee picks one or more tests, and the system creates one request per lab section with a unique barcode (`LR000001` format, Code 39).
- **Technician login:** passwords are stored as SHA-256 hashes, not plain text.
- **Lab explorer:** technicians see only requests from the sections they are certified for.
- **Specimen workflow:** `REQUESTED → SAMPLED → PROCESSED`, or `SAMPLED → REJECTED` with a required reason.
- **Workstation check:** a request can only be handled on a workstation belonging to its section.
- **Results:** technicians enter findings, sign with the mouse (the signature is stored in the database), and release the report.
- **Examinee view:** results are visible only after release, with the barcode and signature.
- **Validation and errors:** invalid input and forbidden status changes show clear messages instead of crashing.

## Architecture (MVC)
| Layer | Folder | Responsibility |
|---|---|---|
| Model | `Models/` | Data classes, repositories and database access |
| View | `Views/` | WinForms screens |
| Controller | `Controllers/` | Validation and business rules |

Data access uses ADO.NET (`MySqlConnection`, `MySqlCommand`, `DataTable`). All queries are parameterised.

## Database
Database name: `LabTestDB`. Tables: `LabSections`, `Examinees`, `Workstations`, `Technicians`, `TechnicianSections`, `LabRequests`. The script is in `Database/labtestdb.sql`.

## Requirements
- Windows 10 or 11
- Visual Studio 2022 or later with the **.NET desktop development** workload
- XAMPP (MySQL/MariaDB, phpMyAdmin)

## How to run
1. Start **Apache** and **MySQL** in the XAMPP Control Panel.
2. Open `http://localhost/phpmyadmin`, create an empty database called `LabTestDB`, and import `Database/labtestdb.sql`.
3. Open `LabTestSystem.slnx` in Visual Studio. Restore NuGet packages if asked.
4. Check the connection string in `Models/AppConfig.cs`:
   `Server=127.0.0.1;Port=3306;Database=LabTestDB;User ID=root;Password=;`
5. Press **F5**.

## Test accounts
| Role | Login |
|---|---|
| Technician 1 (Hematology, Biochemistry) | `tech1` / `tech123` |
| Technician 2 (Microbiology) | `tech2` / `tech123` |
| Examinees | `EX001`, `EX002`, `EX003` |

## Quick walkthrough
1. **Examinees:** add or edit an examinee.
2. **Test Requests:** enter `EX001`, tick the tests, and submit.
3. **Technician:** log in as `tech1`, pick a workstation, select a request, mark it Sampled, then Processed.
4. Type the findings, sign, and click Release Results.
5. **My Results:** enter `EX001` to see the released report.

## Screenshots

### Main menu
<img width="466" height="497" alt="image" src="https://github.com/user-attachments/assets/4f55dc94-b1a3-46c2-bc7a-04ab30820dc2" />


### Examinee management
<img width="687" height="600" alt="image" src="https://github.com/user-attachments/assets/df623c91-724e-404b-a01f-571ff0c9c6be" />


### Submit test request
<img width="896" height="675" alt="image" src="https://github.com/user-attachments/assets/6f3f7e89-7275-4139-b813-d6039b826405" />


### Technician login and lab explorer
<img width="442" height="472" alt="image" src="https://github.com/user-attachments/assets/754ed046-57fc-400e-a6fa-ec45d1b659af" />


### Results with barcode and signature
<img width="867" height="597" alt="image" src="https://github.com/user-attachments/assets/40cabd3d-edb2-4145-a25a-1b7814f666e8" />



### Database (phpMyAdmin)
<img width="1357" height="847" alt="image" src="https://github.com/user-attachments/assets/ae207136-c884-448f-a242-574d8d1c658d" />


## Business rules enforced
- A technician cannot handle a request outside the sections they are certified for.
- Status changes must follow the allowed order; anything else is refused.
- A rejected request must have a reason.
- Results cannot be released without a signature.
- Released results cannot be changed.
- Only requests still in `REQUESTED` status can be deleted.

## Troubleshooting
| Problem | Fix |
|---|---|
| "Unable to connect to any of the specified MySQL hosts" | Start MySQL in XAMPP. |
| "Unknown database 'LabTestDB'" | Create the database and import `Database/labtestdb.sql`. |
| Build errors about missing packages | Right-click the solution → Restore NuGet Packages. |
| Another program uses port 3306 | Change the port in XAMPP and in `AppConfig.cs`. |

## Author
Ndayishimiye Ineza Giuly, Adventist University of Central Africa (AUCA), Faculty of IT
GitHub: [GIULYINEZA2](https://github.com/GIULYINEZA2)
