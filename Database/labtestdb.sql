-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 21, 2026 at 06:37 PM
-- Server version: 10.11.16-MariaDB
-- PHP Version: 8.3.36

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `labtestdb`
--

-- --------------------------------------------------------

--
-- Table structure for table `examinees`
--

CREATE TABLE `examinees` (
  `ExamineeID` int(11) NOT NULL,
  `ExamineeNo` varchar(30) NOT NULL,
  `FullName` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `examinees`
--

INSERT INTO `examinees` (`ExamineeID`, `ExamineeNo`, `FullName`) VALUES
(1, 'EX001', 'Aline Mukamana'),
(2, 'EX002', 'Jean Niyonzima'),
(3, 'EX003', 'Grace Uwase'),
(4, 'EX004', 'Ineza Giuly');

-- --------------------------------------------------------

--
-- Table structure for table `labrequests`
--

CREATE TABLE `labrequests` (
  `RequestID` int(11) NOT NULL,
  `ExamineeID` int(11) NOT NULL,
  `SectionID` int(11) NOT NULL,
  `TestNames` varchar(200) NOT NULL,
  `WorkstationID` int(11) DEFAULT NULL,
  `TechnicianID` int(11) DEFAULT NULL,
  `Status` varchar(20) NOT NULL DEFAULT 'REQUESTED',
  `Notes` varchar(255) DEFAULT NULL,
  `Results` text DEFAULT NULL,
  `Signature` mediumblob DEFAULT NULL,
  `ResultsReleased` tinyint(1) NOT NULL DEFAULT 0,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `labrequests`
--

INSERT INTO `labrequests` (`RequestID`, `ExamineeID`, `SectionID`, `TestNames`, `WorkstationID`, `TechnicianID`, `Status`, `Notes`, `Results`, `Signature`, `ResultsReleased`, `CreatedAt`) VALUES
(1, 1, 2, 'Blood Glucose', NULL, NULL, 'REQUESTED', NULL, NULL, NULL, 0, '2026-11-18 13:35:45');

-- --------------------------------------------------------

--
-- Table structure for table `labsections`
--

CREATE TABLE `labsections` (
  `SectionID` int(11) NOT NULL,
  `SectionName` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `labsections`
--

INSERT INTO `labsections` (`SectionID`, `SectionName`) VALUES
(2, 'Biochemistry'),
(1, 'Hematology'),
(3, 'Microbiology');

-- --------------------------------------------------------

--
-- Table structure for table `technicians`
--

CREATE TABLE `technicians` (
  `TechnicianID` int(11) NOT NULL,
  `Username` varchar(50) NOT NULL,
  `FullName` varchar(100) NOT NULL,
  `PasswordHash` char(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `technicians`
--

INSERT INTO `technicians` (`TechnicianID`, `Username`, `FullName`, `PasswordHash`) VALUES
(1, 'tech1', 'Technician One', '3ac40463b419a7de590185c7121f0bfbe411d6168699e8014f521b050b1d6653'),
(2, 'tech2', 'Technician Two', '3ac40463b419a7de590185c7121f0bfbe411d6168699e8014f521b050b1d6653');

-- --------------------------------------------------------

--
-- Table structure for table `techniciansections`
--

CREATE TABLE `techniciansections` (
  `TechnicianID` int(11) NOT NULL,
  `SectionID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `techniciansections`
--

INSERT INTO `techniciansections` (`TechnicianID`, `SectionID`) VALUES
(1, 1),
(1, 2),
(2, 3);

-- --------------------------------------------------------

--
-- Table structure for table `workstations`
--

CREATE TABLE `workstations` (
  `WorkstationID` int(11) NOT NULL,
  `WorkstationNo` varchar(20) NOT NULL,
  `SectionID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `workstations`
--

INSERT INTO `workstations` (`WorkstationID`, `WorkstationNo`, `SectionID`) VALUES
(1, 'WS-01', 1),
(2, 'WS-02', 2),
(3, 'WS-03', 2),
(4, 'WS-04', 3);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `examinees`
--
ALTER TABLE `examinees`
  ADD PRIMARY KEY (`ExamineeID`),
  ADD UNIQUE KEY `ExamineeNo` (`ExamineeNo`);

--
-- Indexes for table `labrequests`
--
ALTER TABLE `labrequests`
  ADD PRIMARY KEY (`RequestID`),
  ADD KEY `ExamineeID` (`ExamineeID`),
  ADD KEY `SectionID` (`SectionID`),
  ADD KEY `WorkstationID` (`WorkstationID`),
  ADD KEY `TechnicianID` (`TechnicianID`);

--
-- Indexes for table `labsections`
--
ALTER TABLE `labsections`
  ADD PRIMARY KEY (`SectionID`),
  ADD UNIQUE KEY `SectionName` (`SectionName`);

--
-- Indexes for table `technicians`
--
ALTER TABLE `technicians`
  ADD PRIMARY KEY (`TechnicianID`),
  ADD UNIQUE KEY `Username` (`Username`);

--
-- Indexes for table `techniciansections`
--
ALTER TABLE `techniciansections`
  ADD PRIMARY KEY (`TechnicianID`,`SectionID`),
  ADD KEY `SectionID` (`SectionID`);

--
-- Indexes for table `workstations`
--
ALTER TABLE `workstations`
  ADD PRIMARY KEY (`WorkstationID`),
  ADD UNIQUE KEY `WorkstationNo` (`WorkstationNo`),
  ADD KEY `SectionID` (`SectionID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `examinees`
--
ALTER TABLE `examinees`
  MODIFY `ExamineeID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `labrequests`
--
ALTER TABLE `labrequests`
  MODIFY `RequestID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `labsections`
--
ALTER TABLE `labsections`
  MODIFY `SectionID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `technicians`
--
ALTER TABLE `technicians`
  MODIFY `TechnicianID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `workstations`
--
ALTER TABLE `workstations`
  MODIFY `WorkstationID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `labrequests`
--
ALTER TABLE `labrequests`
  ADD CONSTRAINT `labrequests_ibfk_1` FOREIGN KEY (`ExamineeID`) REFERENCES `examinees` (`ExamineeID`),
  ADD CONSTRAINT `labrequests_ibfk_2` FOREIGN KEY (`SectionID`) REFERENCES `labsections` (`SectionID`),
  ADD CONSTRAINT `labrequests_ibfk_3` FOREIGN KEY (`WorkstationID`) REFERENCES `workstations` (`WorkstationID`),
  ADD CONSTRAINT `labrequests_ibfk_4` FOREIGN KEY (`TechnicianID`) REFERENCES `technicians` (`TechnicianID`);

--
-- Constraints for table `techniciansections`
--
ALTER TABLE `techniciansections`
  ADD CONSTRAINT `techniciansections_ibfk_1` FOREIGN KEY (`TechnicianID`) REFERENCES `technicians` (`TechnicianID`),
  ADD CONSTRAINT `techniciansections_ibfk_2` FOREIGN KEY (`SectionID`) REFERENCES `labsections` (`SectionID`);

--
-- Constraints for table `workstations`
--
ALTER TABLE `workstations`
  ADD CONSTRAINT `workstations_ibfk_1` FOREIGN KEY (`SectionID`) REFERENCES `labsections` (`SectionID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
