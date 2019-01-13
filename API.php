<?php

include 'db.php';

switch ($_SERVER["REQUEST_METHOD"])
{
	case "GET": header ("Content-Type: text/json");
	// V případě, že bude aplikace chtít něco získat ze serveru, tak použije metodu GET
	{
		if ($_GET["table"] == "orders" && isset($_GET["parentId"])) {

		    $result = $db -> prepare("SELECT * FROM os_orders WHERE user_id = :user_id");
			$result -> execute(array(':user_id' => $_GET["parentId"]));
			$array = array();
			$arrayIndexes = array();

			foreach ($result as $value) {
				$array[ $value["id"] ] = array("id" => $value["id"], "date" => $value["date"], "visible" => $value["visible"], "items" => array());
			}

		    $result = $db -> prepare("SELECT IG.order_id, IG.item_id, IG.amount, I.name, I.description, I.price FROM os_items_groups IG INNER JOIN os_items I ON IG.item_id=I.id");
			$result -> execute();
			foreach ($result as $value) {
				if (array_key_exists($value["order_id"], $array)) {
					$temp_array = array("id" => $value["item_id"], "name" => $value["name"], "description" => $value["description"], "price" => $value["price"], "amount" => $value["amount"]);
					array_push($array[ $value["order_id"] ]["items"], $temp_array);
				}
			}

			$final_array = array();
			foreach ($array as $value) {
				array_push($final_array, $value);
			}

			echo json_encode ($final_array);
		}
		if ($_GET["table"] == "items") {
		    $result = $db -> prepare("SELECT * FROM os_items WHERE visible = 1");
			$result -> execute();
			$array = array();
			foreach ($result as $value) {
				unset($value[0], $value[1]);
				array_push($array, $value);
			}
			echo json_encode ($array);
		}
		break;
	}
	case 'POST': header ("Content-Type: text/json");
	// V případě, že bude aplikace chtít něco odeslat na server, tak použije metodu POST
	{

		if ($_POST["action"] == "login") {
			$result = $db -> prepare('SELECT * FROM os_users WHERE nick = :nick AND password = :password');
			
			$result -> execute(array(":nick" => $_POST["nick"], ":password" => $_POST["password"]));
			$array = array();
			foreach ($result as $value) {
				array_push($array, $value);
			}
			echo json_encode ($array[0]);

		} else if ($_POST["action"] == "register") {
			$result = $db -> prepare('SELECT COUNT(*) FROM os_users WHERE nick = :nick');
			
			$result -> execute(array(":nick" => $_POST["nick"]));
			foreach ($result as $value) {
				$count = $value["COUNT(*)"];
			}

			$array = array();
			if ($count == 0) {
				$result = $db -> prepare('INSERT INTO os_users (nick,password,name,surname) VALUES (:nick, :password, :name, :surname)');
				$result -> execute(array(":nick" => $_POST["nick"], ":password" => $_POST["password"], ":name" => $_POST["name"], ":surname" => $_POST["surname"]));
				
				$result = $db -> prepare('SELECT * FROM os_users WHERE nick = :nick AND password = :password');
				$result -> execute(array(":nick" => $_POST["nick"], ":password" => $_POST["password"]));
				foreach ($result as $value) {
					array_push($array, $value);
				}
				echo json_encode ($array[0]);
			} else {
				echo json_encode ($array);
			}

		} else if ($_POST["action"] == "updateData") {
			$result = $db -> prepare('UPDATE os_users SET surname = :surname, name = :name WHERE id = :id AND password = :password');
			$result -> execute(array(":id" => $_POST["id"], ":name" => $_POST["name"], ":surname" => $_POST["surname"], ":password" => $_POST["password"]));
			
			$result = $db -> prepare('SELECT * FROM os_users WHERE nick = :nick AND password = :password');
			$result -> execute(array(":nick" => $_POST["nick"], ":password" => $_POST["password"]));
				
			$array = array();
			foreach ($result as $value) {
				array_push($array, $value);
			}
			echo json_encode ($array[0]);

		} else if ($_POST["action"] == "updatePassword") {
			$result = $db -> prepare('UPDATE os_users SET password = :newPassword WHERE id = :id AND password = :password');
			$result -> execute(array(":id" => $_POST["id"], ":password" => $_POST["password"], ":newPassword" => $_POST["newPassword"]));
			
			$result = $db -> prepare('SELECT * FROM os_users WHERE nick = :nick AND password = :newPassword');
			$result -> execute(array(":nick" => $_POST["nick"], ":newPassword" => $_POST["newPassword"]));
				
			$array = array();
			foreach ($result as $value) {
				array_push($array, $value);
			}
			echo json_encode ($array[0]);

		} else if ($_POST["action"] == "addOrder") {
			
			$result = $db -> prepare('INSERT INTO os_orders (user_id,visible) VALUES (:idUser, 1)');
			$result -> execute(array(":idUser" => $_POST["idUser"]));

			$data = $db->query("SELECT LAST_INSERT_ID();");
			foreach ($data as $value) {
				$lastId = $value[0];
			}
			
			$_POST["orderItems"] = json_decode($_POST["orderItems"], true);
			foreach ($_POST["orderItems"] as $item) {
				$result = $db -> prepare('INSERT INTO os_items_groups (order_id, item_id, amount) VALUES (:idOrder, :idItem, :amount)');
				$result -> execute(array(":idOrder" => $lastId, ":idItem" => $item["id"], ":amount" => $item["amount"]));
			}
			$array = array("message" => "Order placed.", "lastId" => $lastId);
			echo json_encode ($array);

		} else if ($_POST["action"] == "hideOrder") {
			
			$result = $db -> prepare('UPDATE os_orders SET visible=0 WHERE id = :idOrder');
			$result -> execute(array(":idOrder" => $_POST["idOrder"]));
			$array = array("message" => "Order was hidden.");
			echo json_encode ($array);

		} else if ($_POST["action"] == "addItem") {
			
			$result = $db -> prepare('INSERT INTO os_items (name,description,price) VALUES (:name, :description, :price)');
			$result -> execute(array(":name" => $_POST["name"], ":description" => $_POST["description"], ":price" => $_POST["price"]));
			$array = array("message" => "Item was added.");
			echo json_encode ($array);

		} else if ($_POST["action"] == "updateItem") {
			
			$result = $db -> prepare('UPDATE os_items SET name=:name, description=:description, price=:price WHERE id = :id');
			$result -> execute(array(":name" => $_POST["name"], ":description" => $_POST["description"], ":price" => $_POST["price"], ":id" => $_POST["id"]));
			$array = array("message" => "Item with id = "  + $_POST["id"] + " was updated.");
			echo json_encode ($array);

		} else if ($_POST["action"] == "deleteItem") {
			
			$result = $db -> prepare('UPDATE os_items SET visible=0 WHERE id = :id');
			$result -> execute(array(":id" => $_POST["id"]));
			$array = array("message" => "Item with id = "  + $_POST["id"] + " was deleted.");
			echo json_encode ($array);

		} else {
			$result = $db -> prepare('INSERT INTO os_users (jmeno,prijmeni,vek) VALUES ($_POST["jmeno"],$_POST["prijmeni"],$_POST["vek"]');
			$result -> execute(array($_POST["jmeno"],$_POST["prijmeni"],$_POST["vek"]));
		}
	    
		break;
	}
	// Dotaz uloží jméno, příjmení a věk do databáze
}

?>