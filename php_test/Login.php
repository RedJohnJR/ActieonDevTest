<?php
include "DBConnect.php";

$username = $_POST['username'];
$password = $_POST['password'];

$stmt = $conn->prepare("SELECT id, password FROM users WHERE username = ?");
$stmt->bind_param("s", $username);
$stmt->execute();
$stmt->store_result();

if ($stmt->num_rows == 0) {
    echo json_encode(["status" => "error", "message" => "Invalid username or password"]);
    exit();
}

$stmt->bind_result($user_id, $hashed_password);
$stmt->fetch();
$stmt->close();

if (!password_verify($password, $hashed_password)) {
    echo json_encode(["status" => "error", "message" => "Invalid username or password"]);
    exit();
}

$stmt = $conn->prepare("SELECT diamond, heart FROM user_data WHERE user_id = ?");
$stmt->bind_param("i", $user_id);
$stmt->execute();
$stmt->bind_result($diamond, $heart);
$stmt->fetch();
$stmt->close();

$stmt = $conn->prepare("INSERT INTO login_history (user_id, login_time) VALUES (?, NOW())");
$stmt->bind_param("i", $user_id);
$stmt->execute();

if ($stmt->affected_rows === 0) {
    echo json_encode(["status" => "error", "message" => "Failed to log login history"]);
    exit();
}

$stmt->close();

echo json_encode([
    "status" => "success",
    "username" => $username,
    "diamond" => $diamond,
    "heart" => $heart
]);

?>
