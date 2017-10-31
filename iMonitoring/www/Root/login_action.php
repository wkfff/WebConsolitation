<?php
	session_start();
	$id = session_id();
	include_once ("scripts.php");
	include_once (getConfigPath());
	include_once ("helperDBData.php");
	include_once ("helperDeviceType.php");
	include_once ("neighborHosts.php");
	$userLogin = $_POST["login"];
	$sql = "SELECT * FROM Users WHERE Name='" . $userLogin . "'";

	//����� �������� �������, ���� � ���� ��� ����� ��������, ��� ���� � ����� TRUE
	$isLookNeighborHost = !isset($_POST['isLookNeighborHost']) || ($_POST['isLookNeighborHost'] == 'TRUE');
	//���� �� ��������� � ��������
	$isMobileUser = !isset($_POST['isMobileUser']) || ($_POST['isMobileUser'] == 'TRUE');
	//� ����� ������ ���������� ������ � ������
	$_SESSION['isNewSnapshotMode'] = ($_POST["isNewSnapshotMode"] == 'TRUE');
	//������ ����������
	$_SESSION['appVersion'] = $_POST['appVersion'];

	//�����������
	$isAdmin = ($userLogin == ADMIN_USER) || ($userLogin == "vybornyh") || ($userLogin == "kotyukovmm")
		|| ($userLogin == "aleksvinnikov") || ($userLogin == "siluanov") || ($userLogin == "kujvashev")
		|| ($userLogin == "maksimovanadeg") || ($userLogin == "izotovags") || ($userLogin == "vubornuh")
		|| ($userLogin == "pshenicinav") || ($userLogin == "mcx1") || ($userLogin == "mcx2")
		|| ($userLogin == "mcx3") || ($userLogin == "isaev_ea") || ($userLogin == "shytenkov")
		|| ($userLogin == "aiibragimov") || ($userLogin == "ramzan_kadyrov") || ($userLogin == "ditjatevpv")
		|| ($userLogin == "gov") || ($userLogin == "cheba") || ($userLogin == "baidin")
		|| ($userLogin == "komarova") || ($userLogin == "petrovis") || ($userLogin == "gripasva")
		|| ($userLogin == "putinaa") || ($userLogin == "buhtingf") || ($userLogin == "dudinava")
		|| ($userLogin == "ermoshinva") || ($userLogin == "kim") || ($userLogin == "pryamilova")
		|| ($userLogin == "kiligekovya") || ($userLogin == "svincovaap") || ($userLogin == "nelidov")
		|| ($userLogin == "usynin_iv") || ($userLogin == "efremov") || ($userLogin == "ivanov")
		|| ($userLogin == "vip") || ($userLogin == "yudaevaov") || ($userLogin == "fedorov-yar")
		|| ($userLogin == "gaevskyvv") || ($userLogin == "ivanovpa") || ($userLogin == "eroshkina")
		|| ($userLogin == "vvartjakov") || ($userLogin == "koshelevag") || ($userLogin == "mvaksman")
		|| ($userLogin == "tver1") || ($userLogin == "tver2") || ($userLogin == "tver3")
		|| ($userLogin == "tver4") || ($userLogin == "tver5") || ($userLogin == "tver6")
		|| ($userLogin == "tver7") || ($userLogin == "tver8") || ($userLogin == "tver9")
		|| ($userLogin == "tver10") || ($userLogin == "tver11") || ($userLogin == "tver12")
		|| ($userLogin == "tver13") || ($userLogin == "tver14") || ($userLogin == "tver15")
		|| ($userLogin == "gordeev") || ($userLogin == "safonova") || ($userLogin == "tver")
		|| ($userLogin == "urchenko") || ($userLogin == "gornin") || ($userLogin == "tugarin")
		|| ($userLogin == "lavrov") || ($userLogin == "kotyakov") || ($userLogin == "shapovalov")
		|| ($userLogin == "pryamilovav") || ($userLogin == "tapsiev") || ($userLogin == "larionov")
		|| ($userLogin == "polyakov") || ($userLogin == "tolokonsky") || ($userLogin == "san")
		|| ($userLogin == "vahrukov") || ($userLogin == "markelov") || ($userLogin == "dudnichenko")
		|| ($userLogin == "sidorov") || ($userLogin == "doronin") || ($userLogin == "bezhaev")
		|| ($userLogin == "fedenevalm") || ($userLogin == "kirin_dn") || ($userLogin == "kotjukovmm")
		|| ($userLogin == "usachevaey") || ($userLogin == "maliginaos") || ($userLogin == "saratovva")
		|| ($userLogin == "gvozdevanm") || ($userLogin == "vologda") || ($userLogin == "bushmin")
		|| ($userLogin == "vologda1") || ($userLogin == "vologda2") || ($userLogin == "yuriev")
		|| ($userLogin == "krista") || ($userLogin == "nesterenko") || ($userLogin == "zelenev") 
		|| ($userLogin == "kudrin") || ($userLogin == "vubornuh") || ($userLogin == "atukova")
		|| ($userLogin == "lebedev") || ($userLogin == "smirnov") || ($userLogin == "sochi")
		|| ($userLogin == "omsk1") || ($userLogin == "mincomsvyaz") || ($userLogin == "novak")
		|| ($userLogin == "penza") || ($userLogin == "urfo") || ($userLogin == "kostroma")
		|| ($userLogin == "krikynovatm") || ($userLogin == "mcx1") || ($userLogin == "mcx2")
		|| ($userLogin == "mcx3") || ($userLogin == "mejuev") || ($userLogin == "gruzdev");

	$_SESSION['isAdmin'] = $isAdmin;
	$isAuthorizationByGroup = isset($_POST['authorizedGroups']);
	if ($isAuthorizationByGroup)
		$_SESSION['authorizedGroups'] = $_POST['authorizedGroups'];

	//���� ������������ �� ������ ��������������
	$_SESSION['isAuthentication'] = FALSE;
	$result = mysql_query($sql);
	$row = mysql_fetch_assoc($result);
	if ($row || $isAdmin || $isAuthorizationByGroup)
	{
		$basePassword = $row["PwdHashSHA"];
		$userPassword = $_POST["password"];

		$sessionPassword = getSessionPassword($basePassword, $id);
		
		unset($_SESSION['userInfo']);
		if (($sessionPassword == $userPassword) || $isAdmin || $isAuthorizationByGroup)
		{
			$_SESSION["userID"] = $isAdmin || !isset($row["ID"]) ? "0" : $row["ID"];
			$_SESSION["userLogin"] = $userLogin;
			
			$deviceType = getMobileDeviceType($_POST['mobileDeviceType']);
			$_SESSION["mobileDeviceType"] = $deviceType;
			
			//�������� ���������� � ������� �����
			$currentHostInfo = new HostInfo();
			//��������� ���� ��� �������� �����
			$currentHostInfo->Uri = getCurrentUriPath();
			$currentHostInfo->initSelfAvailableUserReports();
			$currentHostInfo->SessionID = $id;
			$currentHostInfo->setCookie(TRUE);
			
			$_SESSION["accessReports"] = $currentHostInfo->getArrayReports();
			
			//���� ����� �������
			if ($isLookNeighborHost)
			{	
				$neighborHosts = new NeighborHosts();
				$neighborHosts->loadHosts(getFullFilePath(NEIGHBOR_HOSTS_FILE_NAME));
				$neighborHosts->initHosts($userLogin, $basePassword);
				//������ ������������� ������, ������������ �������� ������...
				validReports($currentHostInfo, $neighborHosts);
				//�.�. ��� ������������� �������� �������� ���� � ����� �������, 
				//� �� � ��������, ����� ���������� ���� � id ������, ��� �� ��� 
				//��������� �������� - �� �������, �������� ������ ������ ��� �� "������".
				$neighborHosts->setCookies();
				$_SESSION["neighborHosts"] = gzcompress(serialize($neighborHosts));
			}
			$_SESSION["currentHostInfo"] = gzcompress(serialize($currentHostInfo));
			$_SESSION['isAuthentication'] = TRUE;
			//������� � ���������� �����, ��� � ��� �������
			updateAuthorizationStatistics($userLogin, getIP(), $deviceType, $_SESSION['appVersion']);
			//������� ������ � ������ ����������
			updateActualSession($id);
			if (($deviceType == MobileDeviceTypes::IPAD) && ($_SESSION['appVersion'] == ''))
				$_SESSION['userInfo'] = win1251ToUtf8("�������� ����� ������ ����������. ����� �����������, ����� ������� � ���������� ������. ����������� �������� ���������� i����������");		
		}
		else
		{
			$_SESSION['userInfo'] = win1251ToUtf8("������ �� ������������� ����� ������������");	
		}
	}
	else 
	{	
		$_SESSION['userInfo'] = win1251ToUtf8('�� ���������� ������������ � ������: ') . $userLogin;
	}
	unset($row);
	
	if ($isMobileUser)
		header("Location: userSettings.php");
?>