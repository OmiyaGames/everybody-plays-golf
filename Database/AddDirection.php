<?php 
        $db = mysql_connect('omiyagamescom.fatcowmysql.com', 'mysql_user', 'mysql_password') or die('Could not connect: ' . mysql_error()); 
        mysql_select_db('my_database') or die('Could not select database');
 
        // Strings must be escaped to prevent SQL injection attack. 
        $time = mysql_real_escape_string($_POST['time'], $db); 
        $x = mysql_real_escape_string($_POST['x'], $db);
        $z = mysql_real_escape_string($_POST['z'], $db);
        $hash = $_POST['hash']; 
 
        $secretKey="mySecretKey"; # Change this value to match the value stored in the client javascript below 

        $real_hash = md5($name . $score . $secretKey); 
        if($real_hash == $hash) { 
            // Send variables for the MySQL database class. 
            $query = "insert into scores values (NULL, '$name', '$score');"; 
            $result = mysql_query($query) or die('Query failed: ' . mysql_error()); 
        } 
?>
