# Lab Test Request and Results Management System

**Student:** Ndayishimiye Ineza Giuly (27390)
**Course:** .NET, Lab 3
**Stack:** C# WinForms, MVC, MariaDB (XAMPP), MySqlConnector

## Setup
1. Start **Apache** and **MySQL** in XAMPP.
2. In phpMyAdmin, import `Database/labtestdb.sql`. If `LabTestDB` does not exist yet, create an empty database with that name first.
3. Open `LabTestSystem.slnx` in Visual Studio and press **F5**.

## Test accounts
| Role | Login |
|---|---|
| Technician 1 (Hematology, Biochemistry) | `tech1` / `tech123` |
| Technician 2 (Microbiology) | `tech2` / `tech123` |
| Examinees | `EX001`, `EX002`, `EX003` |

## Project structure
- **Models:** data classes and database access (ADO.NET)
- **Views:** WinForms screens
- **Controllers:** validation and business rules

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
