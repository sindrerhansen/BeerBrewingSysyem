<?php
$con = mysql_connect("localhost","root","Kaffe123");

if (!$con) {
  die('Could not connect: ' . mysql_error());
}

mysql_select_db("bryggelogg", $con);

$result = mysql_query("SELECT * FROM b8_pale_ale");

while($row = mysql_fetch_array($result)) {
  echo $row['date_enterd'] . "\t" . $row['hwtTemp']. "\t" . $row['mashTankTemp']. "\t" . $row['boilTankTemp'].  "\t" . $row['addedVolume']."\n";
}
mysql_close($con);
?> 