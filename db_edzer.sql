-- phpMyAdmin SQL Dump
-- version 4.9.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Dec 13, 2019 at 05:34 PM
-- Server version: 10.4.8-MariaDB
-- PHP Version: 7.3.11

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `db_edzer`
--

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `ClearAllRecords` ()  BEGIN
SET FOREIGN_KEY_CHECKS=0;
TRUNCATE tbl_instrumentrented;
TRUNCATE tbl_schedule;
TRUNCATE tbl_instrument;
TRUNCATE tbl_band;
TRUNCATE tbl_representative;
SET FOREIGN_KEY_CHECKS=1;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_band`
--

CREATE TABLE `tbl_band` (
  `BandID` int(11) NOT NULL,
  `BandName` varchar(40) NOT NULL,
  `HasPenalty` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tbl_band`
--

INSERT INTO `tbl_band` (`BandID`, `BandName`, `HasPenalty`) VALUES
(1, 'Band01', 0),
(2, 'Band02', 1),
(3, 'Band03', 0);

-- --------------------------------------------------------

--
-- Table structure for table `tbl_instrument`
--

CREATE TABLE `tbl_instrument` (
  `InstrumentID` int(11) NOT NULL,
  `InstrumentModel` varchar(20) NOT NULL,
  `InstrumentType` varchar(20) NOT NULL,
  `InstrumentDescription` varchar(100) DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tbl_instrument`
--

INSERT INTO `tbl_instrument` (`InstrumentID`, `InstrumentModel`, `InstrumentType`, `InstrumentDescription`, `IsDeleted`) VALUES
(1, 'DS101', 'DRUM STICK', 'DS Gold', 0),
(2, 'EB12', 'ELECTRIC BASS', 'Bass 2015', 0);

-- --------------------------------------------------------

--
-- Table structure for table `tbl_instrumentrented`
--

CREATE TABLE `tbl_instrumentrented` (
  `FK_InstrumentType_InstrumentType` varchar(20) NOT NULL,
  `FK_Schedule_ScheduleID` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tbl_instrumentrented`
--

INSERT INTO `tbl_instrumentrented` (`FK_InstrumentType_InstrumentType`, `FK_Schedule_ScheduleID`, `Quantity`) VALUES
('DRUM STICK', 3, 1),
('DRUM STICK', 1, 1),
('DRUM STICK', 5, 1),
('ELECTRIC BASS', 5, 1);

-- --------------------------------------------------------

--
-- Table structure for table `tbl_instrumenttype`
--

CREATE TABLE `tbl_instrumenttype` (
  `InstrumentType` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tbl_instrumenttype`
--

INSERT INTO `tbl_instrumenttype` (`InstrumentType`) VALUES
('DRUM STICK'),
('ELECTRIC BASS'),
('ELECTRIC GUITAR'),
('FX/STOMPBOX');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_instrumenttypeprice`
--

CREATE TABLE `tbl_instrumenttypeprice` (
  `InstrumentType` varchar(20) NOT NULL,
  `PricePerHour` float NOT NULL,
  `DateEffective` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tbl_instrumenttypeprice`
--

INSERT INTO `tbl_instrumenttypeprice` (`InstrumentType`, `PricePerHour`, `DateEffective`) VALUES
('DRUM STICK', 20, '2019-01-01 00:00:00'),
('ELECTRIC BASS', 20, '2019-01-01 00:00:00'),
('ELECTRIC GUITAR', 20, '2019-01-01 00:00:00'),
('FX/STOMPBOX', 20, '2019-01-01 00:00:00');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_representative`
--

CREATE TABLE `tbl_representative` (
  `RepresentativeName` varchar(50) NOT NULL,
  `ContactNumber` varchar(13) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tbl_representative`
--

INSERT INTO `tbl_representative` (`RepresentativeName`, `ContactNumber`) VALUES
('Rep01', '0901010101'),
('Rep02', '0902020202'),
('Rep03', '090303030303');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_schedule`
--

CREATE TABLE `tbl_schedule` (
  `ScheduleID` int(11) NOT NULL,
  `FK_Band_BandID` int(11) NOT NULL,
  `FK_ScheduleType_ScheduleTypeID` int(11) NOT NULL,
  `FK_Representative_RepresentativeName` varchar(50) DEFAULT NULL,
  `DateAdded` date NOT NULL,
  `ScheduleDate` date NOT NULL,
  `StartTime` time NOT NULL,
  `DurationInHours` int(11) NOT NULL,
  `IsPaid` tinyint(1) NOT NULL DEFAULT 0,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tbl_schedule`
--

INSERT INTO `tbl_schedule` (`ScheduleID`, `FK_Band_BandID`, `FK_ScheduleType_ScheduleTypeID`, `FK_Representative_RepresentativeName`, `DateAdded`, `ScheduleDate`, `StartTime`, `DurationInHours`, `IsPaid`, `IsDeleted`) VALUES
(1, 1, 2, 'Rep01', '2019-12-06', '2019-12-13', '08:00:00', 2, 1, 1),
(2, 2, 3, 'Rep02', '2019-12-06', '2019-12-06', '06:00:00', 1, 0, 0),
(3, 3, 5, 'Rep03', '2019-12-06', '2019-12-12', '10:00:00', 2, 0, 0),
(4, 2, 1, 'Rep02', '2019-12-13', '2019-12-13', '22:00:00', 2, 1, 0),
(5, 1, 4, 'Rep01', '2019-12-13', '2019-12-14', '01:00:00', 1, 1, 0);

-- --------------------------------------------------------

--
-- Table structure for table `tbl_scheduletype`
--

CREATE TABLE `tbl_scheduletype` (
  `ScheduleTypeID` int(11) NOT NULL,
  `ScheduleTypeName` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tbl_scheduletype`
--

INSERT INTO `tbl_scheduletype` (`ScheduleTypeID`, `ScheduleTypeName`) VALUES
(5, 'DRUMS'),
(4, 'INSTRUMENTS'),
(2, 'LIVE FULL BAND'),
(1, 'STUDIO'),
(6, 'STUDIO - STUDENT'),
(3, 'VOICE');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_scheduletypeprice`
--

CREATE TABLE `tbl_scheduletypeprice` (
  `ScheduleTypeID` int(11) NOT NULL,
  `PricePerHour` float NOT NULL,
  `DateEffective` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `tbl_scheduletypeprice`
--

INSERT INTO `tbl_scheduletypeprice` (`ScheduleTypeID`, `PricePerHour`, `DateEffective`) VALUES
(1, 200, '2019-01-01 00:00:00'),
(2, 799, '2019-01-01 00:00:00'),
(3, 499, '2019-01-01 00:00:00'),
(4, 499, '2019-01-01 00:00:00'),
(5, 599, '2019-01-01 00:00:00'),
(6, 180, '2019-01-01 00:00:00');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_user`
--

CREATE TABLE `tbl_user` (
  `UserID` int(11) NOT NULL,
  `Username` varchar(20) NOT NULL,
  `PasswordHash` varchar(40) NOT NULL,
  `IsAdmin` tinyint(1) NOT NULL DEFAULT 0,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tbl_user`
--

INSERT INTO `tbl_user` (`UserID`, `Username`, `PasswordHash`, `IsAdmin`, `IsDeleted`) VALUES
(1, 'admin', 'd033e22ae348aeb5660fc2140aec35850c4da997', 1, 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tbl_band`
--
ALTER TABLE `tbl_band`
  ADD PRIMARY KEY (`BandID`);

--
-- Indexes for table `tbl_instrument`
--
ALTER TABLE `tbl_instrument`
  ADD PRIMARY KEY (`InstrumentID`),
  ADD KEY `InstrumentType` (`InstrumentType`);

--
-- Indexes for table `tbl_instrumentrented`
--
ALTER TABLE `tbl_instrumentrented`
  ADD KEY `FK_InstrumentType_InstrumentType` (`FK_InstrumentType_InstrumentType`),
  ADD KEY `FK_Schedule_ScheduleID` (`FK_Schedule_ScheduleID`);

--
-- Indexes for table `tbl_instrumenttype`
--
ALTER TABLE `tbl_instrumenttype`
  ADD PRIMARY KEY (`InstrumentType`);

--
-- Indexes for table `tbl_instrumenttypeprice`
--
ALTER TABLE `tbl_instrumenttypeprice`
  ADD KEY `InstrumentType` (`InstrumentType`);

--
-- Indexes for table `tbl_representative`
--
ALTER TABLE `tbl_representative`
  ADD PRIMARY KEY (`RepresentativeName`);

--
-- Indexes for table `tbl_schedule`
--
ALTER TABLE `tbl_schedule`
  ADD PRIMARY KEY (`ScheduleID`),
  ADD KEY `FK_Band_BandID` (`FK_Band_BandID`),
  ADD KEY `FK_Representative_RepresentativeName` (`FK_Representative_RepresentativeName`),
  ADD KEY `FK_ScheduleType_ScheduleTypeID` (`FK_ScheduleType_ScheduleTypeID`);

--
-- Indexes for table `tbl_scheduletype`
--
ALTER TABLE `tbl_scheduletype`
  ADD PRIMARY KEY (`ScheduleTypeID`),
  ADD UNIQUE KEY `ScheduleTypeName` (`ScheduleTypeName`);

--
-- Indexes for table `tbl_scheduletypeprice`
--
ALTER TABLE `tbl_scheduletypeprice`
  ADD KEY `ScheduleTypeID` (`ScheduleTypeID`);

--
-- Indexes for table `tbl_user`
--
ALTER TABLE `tbl_user`
  ADD PRIMARY KEY (`UserID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tbl_band`
--
ALTER TABLE `tbl_band`
  MODIFY `BandID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `tbl_instrument`
--
ALTER TABLE `tbl_instrument`
  MODIFY `InstrumentID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `tbl_schedule`
--
ALTER TABLE `tbl_schedule`
  MODIFY `ScheduleID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `tbl_scheduletype`
--
ALTER TABLE `tbl_scheduletype`
  MODIFY `ScheduleTypeID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `tbl_scheduletypeprice`
--
ALTER TABLE `tbl_scheduletypeprice`
  MODIFY `ScheduleTypeID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `tbl_user`
--
ALTER TABLE `tbl_user`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `tbl_instrument`
--
ALTER TABLE `tbl_instrument`
  ADD CONSTRAINT `tbl_instrument_ibfk_1` FOREIGN KEY (`InstrumentType`) REFERENCES `tbl_instrumenttype` (`InstrumentType`);

--
-- Constraints for table `tbl_instrumentrented`
--
ALTER TABLE `tbl_instrumentrented`
  ADD CONSTRAINT `tbl_instrumentrented_ibfk_1` FOREIGN KEY (`FK_InstrumentType_InstrumentType`) REFERENCES `tbl_instrumenttype` (`InstrumentType`),
  ADD CONSTRAINT `tbl_instrumentrented_ibfk_2` FOREIGN KEY (`FK_Schedule_ScheduleID`) REFERENCES `tbl_schedule` (`ScheduleID`);

--
-- Constraints for table `tbl_instrumenttypeprice`
--
ALTER TABLE `tbl_instrumenttypeprice`
  ADD CONSTRAINT `tbl_instrumenttypeprice_ibfk_1` FOREIGN KEY (`InstrumentType`) REFERENCES `tbl_instrumenttype` (`InstrumentType`);

--
-- Constraints for table `tbl_schedule`
--
ALTER TABLE `tbl_schedule`
  ADD CONSTRAINT `FK_ScheduleType_ScheduleTypeID` FOREIGN KEY (`FK_ScheduleType_ScheduleTypeID`) REFERENCES `tbl_scheduletype` (`ScheduleTypeID`),
  ADD CONSTRAINT `tbl_schedule_ibfk_1` FOREIGN KEY (`FK_Band_BandID`) REFERENCES `tbl_band` (`BandID`),
  ADD CONSTRAINT `tbl_schedule_ibfk_2` FOREIGN KEY (`FK_Representative_RepresentativeName`) REFERENCES `tbl_representative` (`RepresentativeName`);

--
-- Constraints for table `tbl_scheduletypeprice`
--
ALTER TABLE `tbl_scheduletypeprice`
  ADD CONSTRAINT `tbl_scheduletypeprice_ibfk_1` FOREIGN KEY (`ScheduleTypeID`) REFERENCES `tbl_scheduletype` (`ScheduleTypeID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
