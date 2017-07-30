<?php 
        $db = mysql_connect('mysql_host', 'mysql_user', 'mysql_password') or die('Could not connect: ' . mysql_error()); 
        mysql_select_db('my_database') or die('Could not select database');
 
        // Strings must be escaped to prevent SQL injection attack. 
        $ids = mysql_real_escape_string($_POST['ids'], $db);
        $hash = $_POST['hash']; 
 
        $secretKey="mySecretKey"; # Change this value to match the value stored in the client javascript below 

        $real_hash = md5($ids . $secretKey);
        if($real_hash == $hash) {
            // Send variables for the MySQL database class. 
            $query = "DELETE from directions WHERE id IN ($id);"; 
            $result = mysql_query($query) or die('Query failed: ' . mysql_error()); 
        } else {
			die('hash did not match'); 
		}
?>
