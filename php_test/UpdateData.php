<?php
include "DBConnect.php";

if (!isset($_POST['username']) || !isset($_POST['diamond']) || !isset($_POST['heart'])) {
    echo json_encode(["status" => "error", "message" => "Missing parameters"]);
    exit();
}

$username = $_POST['username'];
$diamond = intval($_POST['diamond']);
$heart = intval($_POST['heart']);

$stmt = $conn->prepare("SELECT id FROM users WHERE username = ?");
$stmt->bind_param("s", $username);
$stmt->execute();
$stmt->store_result();

if ($stmt->num_rows == 0) {
    echo json_encode(["status" => "error", "message" => "User not found"]);
    exit();
}

$stmt->bind_result($user_id);
$stmt->fetch();
$stmt->close();

$maxDiamond = 10000;
$maxHeart = 100;

$diamond = max(0, min($diamond, $maxDiamond));
$heart = max(0, min($heart, $maxHeart));

error_log("Updating user_id: $user_id | Diamond: $diamond | Heart: $heart");

$stmt = $conn->prepare("UPDATE user_data SET diamond = ?, heart = ? WHERE user_id = ?");
$stmt->bind_param("iii", $diamond, $heart, $user_id);
$stmt->execute();

if ($stmt->affected_rows === 0) {
    echo json_encode(["status" => "error", "message" => "No changes made"]);
    exit();
}

$stmt = $conn->prepare("SELECT diamond, heart FROM user_data WHERE user_id = ?");
$stmt->bind_param("i", $user_id);
$stmt->execute();
$stmt->bind_result($updatedDiamond, $updatedHeart);
$stmt->fetch();
$stmt->close();

echo json_encode(["status" => "success", "message" => "Update successful", "diamond" => $updatedDiamond, "heart" => $updatedHeart]);

?>
