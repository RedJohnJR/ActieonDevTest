<?php
$servername = "103.91.190.179";  
$username = "testdev07";
$password = "w7Lj6gJXsjBMvhtJseS7kEW7QZ8sifCn"; 
$dbname = "testdev07";

$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

mysqli_set_charset($conn, "utf8");

?>
