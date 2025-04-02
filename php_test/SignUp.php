<?php
include "DBConnect.php";

$username = $_POST['username'];
$password = password_hash($_POST['password'], PASSWORD_BCRYPT);

$stmt = $conn->prepare("SELECT id FROM users WHERE username = ?");
$stmt->bind_param("s", $username);
$stmt->execute();
$stmt->store_result();

if ($stmt->num_rows > 0) {
    echo json_encode(["status" => "error", "message" => "Username already exists"]);
    exit();
}

$stmt = $conn->prepare("INSERT INTO users (username, password) VALUES (?, ?)");
$stmt->bind_param("ss", $username, $password);
$stmt->execute();

$user_id = $stmt->insert_id;
$conn->query("INSERT INTO user_data (user_id, diamond, heart) VALUES ($user_id, 1000, 100)");

echo json_encode(["status" => "success", "message" => "Signup successful"]);
?>
