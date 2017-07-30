<?php
    // Send variables for the MySQL database class.
    $database = mysql_connect('mysql_host', 'mysql_user', 'mysql_password') or die('Could not connect: ' . mysql_error());
    mysql_select_db('my_database') or die('Could not select database');

    $query = "SELECT * FROM `directions` WHERE `read` = 0 ORDER by `time` LIMIT 10;";
	
    $result = mysql_query($query) or die('Query failed: ' . mysql_error());
    $num_results = mysql_num_rows($result);  
	$divider = '|';
 
    for($i = 0; $i < $num_results; $i++)
    {
         $row = mysql_fetch_array($result);
         echo $row['id'] . $divider . $row['time'] . $divider . $row['x'] . $divider . $row['z'] . $divider . $row['name'] . "\n";
    }
?>
