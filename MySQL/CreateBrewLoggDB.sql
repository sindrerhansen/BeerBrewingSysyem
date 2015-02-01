CREATE DATABASE `bryggelogg` /*!40100 DEFAULT CHARACTER SET utf8 */;
CREATE TABLE `bryggelogg.testbrygg` (
  `idbryggnrtest` int(11) NOT NULL AUTO_INCREMENT,
  `addedVolume` float NOT NULL,
  `hwtTemp` float NOT NULL,
  `mashTankTemp` float NOT NULL,
  `boilTankTemp` float NOT NULL,
  `date_enterd` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `state` float NOT NULL,
  PRIMARY KEY (`idbryggnrtest`),
  UNIQUE KEY `idbryggnrtest_UNIQUE` (`idbryggnrtest`)
) ENGINE=InnoDB AUTO_INCREMENT=1442 DEFAULT CHARSET=utf8;
