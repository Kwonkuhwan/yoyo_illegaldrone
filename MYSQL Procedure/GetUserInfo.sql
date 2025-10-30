# 유저 정보 불러오기
# 프로시저 삭제
Drop Procedure GetUserInfo;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetUserInfo;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetUserInfo(
    IN in_id VARCHAR(255),
    OUT return_state INT
)
BEGIN
    DECLARE usergroup INT;

    -- 예외 발생 시 처리
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;  -- 실패 상태
    END;

    -- 사용자의 정보를 확인합니다.    
    SELECT u.Usergroup INTO usergroup 
	FROM lllegaldrone.userdb u
	WHERE u.ID = in_id;    
    
    IF usergroup = 2 THEN
		SELECT 
			u.Usergroup, 
			u.ID, 
			u.PW, 
			u.UserName, 
			u.Gender, 
			u.CreateIDDate,
			 CASE 
				WHEN u.DeleteIDDate IS NULL THEN 'false'
				WHEN DeleteIDDate < NOW() THEN 'true' ELSE 'false' 
			END AS DeleteID,
			u.EndAccessDate, 
			u.TotalPlayTime, 
			u.ModifyPW, 
			u.Totorialcomplete, 
			COUNT(p.UserID) AS MissionResult_count, -- scenarioDB에서 CreateID 개수 세기
			COUNT(CASE WHEN p.TeamMissionResult = 1 THEN p.UserID END) AS teamMissionResult_count, -- TeamMissionResult가 1인 경우만 카운트  
				CASE 
				WHEN COUNT(p.UserID) = 0 THEN 0 -- MissionResult_count가 0일 경우 방지
				ELSE (COUNT(CASE WHEN p.TeamMissionResult = 1 THEN p.UserID END) * 100.0 / COUNT(p.UserID)) 
			END AS Result_percentage, -- 퍼센트 계산
			u.CreateAdminName
		FROM 
			lllegaldrone.userdb u
		LEFT JOIN 
			playdb p On u.ID = p.UserID
		WHERE 
			u.ID = in_id
			AND u.Usergroup IN (2) -- Usergroup이 0 또는 1인 경우
		GROUP BY 
			u.Usergroup, u.ID, u.PW, u.UserName, u.Gender, u.CreateIDDate,
			u.DeleteIDDate, u.EndAccessDate, u.TotalPlayTime, u.ModifyPW,
			u.Totorialcomplete, u.CreateAdminName;
    ELSE
		SELECT 
			u.Usergroup, 
			u.ID, 
			u.PW, 
			u.UserName, 
			u.Gender, 
			u.CreateIDDate,
			 CASE 
				WHEN u.DeleteIDDate IS NULL THEN 'false'
				WHEN DeleteIDDate < NOW() THEN 'true' ELSE 'false' 
			END AS DeleteID,
			u.EndAccessDate, 
			u.TotalPlayTime, 
			u.ModifyPW, 
			u.Totorialcomplete, 
			COUNT(p.UserID) AS MissionResult_count, -- scenarioDB에서 CreateID 개수 세기
			COUNT(CASE WHEN p.TeamMissionResult = 1 THEN p.UserID END) AS teamMissionResult_count, -- TeamMissionResult가 1인 경우만 카운트  
				CASE 
				WHEN COUNT(p.UserID) = 0 THEN 0 -- MissionResult_count가 0일 경우 방지
				ELSE (COUNT(CASE WHEN p.TeamMissionResult = 1 THEN p.UserID END) * 100.0 / COUNT(p.UserID)) 
			END AS Result_percentage, -- 퍼센트 계산
			u.CreateAdminName
		FROM 
			lllegaldrone.userdb u
		LEFT JOIN 
			playdb p On u.ID = p.UserID
		WHERE 
			u.ID = in_id
			AND u.Usergroup IN (0, 1) -- Usergroup이 0 또는 1인 경우
		GROUP BY 
			u.Usergroup, u.ID, u.PW, u.UserName, u.Gender, u.CreateIDDate,
			u.DeleteIDDate, u.EndAccessDate, u.TotalPlayTime, u.ModifyPW,
			u.Totorialcomplete, u.CreateAdminName;
    END IF;        
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL GetUserInfo('SMW', @return_state);
SELECT @return_state;

CALL GetUserInfo('123456789', @return_state);

SELECT 
    u.Usergroup, 
    u.ID, 
    u.PW, 
    u.UserName, 
    u.Gender, 
    u.CreateIDDate,
    CASE 
        WHEN u.DeleteIDDate IS NULL THEN 'false'
        WHEN DeleteIDDate < NOW() THEN TRUE ELSE FALSE 
    END AS DeleteID,
    u.EndAccessDate, 
    u.TotalPlayTime, 
    u.ModifyPW, 
    u.Totorialcomplete, 
    COUNT(p.UserID) AS MissionResult_count, -- scenarioDB에서 CreateID 개수 세기
    COUNT(CASE WHEN p.TeamMissionResult = 1 THEN p.UserID END) AS teamMissionResult_count, -- TeamMissionResult가 1인 경우만 카운트  
        CASE 
        WHEN COUNT(p.UserID) = 0 THEN 0 -- MissionResult_count가 0일 경우 방지
        ELSE (COUNT(CASE WHEN p.TeamMissionResult = 1 THEN p.UserID END) * 100.0 / COUNT(p.UserID)) 
    END AS Result_percentage, -- 퍼센트 계산
    u.CreateAdminName
FROM 
    lllegaldrone.userdb u
LEFT JOIN 
	playdb p On u.ID = p.UserID
WHERE 
    u.ID = "12"
    AND u.Usergroup IN (2) -- Usergroup이 0 또는 1인 경우
GROUP BY 
    u.Usergroup, u.ID, u.PW, u.UserName, u.Gender, u.CreateIDDate,
    u.DeleteIDDate, u.EndAccessDate, u.TotalPlayTime, u.ModifyPW,
    u.Totorialcomplete, u.CreateAdminName;
    
    select * from userdb;
    
    select DATEDIFF(CURRENT_DATE, "2024-10-16 11:54:09") < ;
    
    SELECT 
    CASE 
        WHEN DeleteIDDate < NOW() THEN TRUE 
        ELSE FALSE 
    END AS is_before_now
FROM 
    userdb
    where ID = "yoyo1234";