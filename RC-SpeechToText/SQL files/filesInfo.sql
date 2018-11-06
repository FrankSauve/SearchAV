CREATE TABLE `SearchAV`.`filesInfo` (
  `Url` VARCHAR(45) NOT NULL,
  `Transcript` varbinary(max) NOT NULL,
  `VerifiedVal` bit NOT NULL, 
  `Author` VARCHAR(45) NOT NULL, 
  `Type` VARCHAR(5) NOT NULL, 
  PRIMARY KEY (`Url`));
  