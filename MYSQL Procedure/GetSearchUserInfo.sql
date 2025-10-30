# 유저 정보 불러오기
# 프로시저 삭제
Drop Procedure GetSearchUserInfo;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetSearchUserInfo;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetSearchUserInfo(
    IN in_offset INT,
    IN in_maxCnt INT,
    IN in_asc BOOL,
	IN in_name VARCHAR(255),
    IN in_issearch BOOL,
    IN in_userGroup INT,
    OUT return_state INT
)
BEGIN    
	DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;  -- 실패 상태
    END;
    
    SET @sql = '';    

    IF in_issearch THEN
        SET @sql = CONCAT('
            SELECT                
				ROW_NUMBER() OVER (ORDER BY MIN(u.CreateIDDate) ASC) AS idx,
				u.CreateIDDate, 
				u.ID, 
				u.UserName, 
				COALESCE(COUNT(p.UserID), 0) AS Scenariocount,
				COALESCE(MAX(p.PlayScenarioDateTime), ''1999-01-01 00:00:00'') AS PlayScenarioDateTime,
				u.CreateAdminName 
			FROM 
				lllegaldrone.userdb u
			LEFT JOIN 
				lllegaldrone.playdb p on p.userID = u.ID
            WHERE 
                u.UserName LIKE CONCAT("%", ?, "%") and u.Usergroup = ?
            GROUP BY 
                u.CreateIDDate, u.ID, u.UserName, u.CreateAdminName
            ORDER BY 
                MIN(u.CreateIDDate) ', IF(in_asc, 'ASC', 'DESC'), '
            LIMIT ? OFFSET ?;
        ');
        PREPARE stmt FROM @sql;
        SET @name = in_name;
        SET @userGroup = in_userGroup;
        SET @maxCnt = in_maxCnt;
        SET @offset = in_offset;
        EXECUTE stmt USING @name, @userGroup, @maxCnt, @offset;
        DEALLOCATE PREPARE stmt;
    ELSE
        SET @sql = CONCAT('
            SELECT                
				ROW_NUMBER() OVER (ORDER BY MIN(u.CreateIDDate) ASC) AS idx,
				u.CreateIDDate, 
				u.ID, 
				u.UserName, 
				COALESCE(COUNT(p.UserID), 0) AS Scenariocount,
				COALESCE(MAX(p.PlayScenarioDateTime), ''1999-01-01 00:00:00'') AS PlayScenarioDateTime,
				u.CreateAdminName 
			FROM 
				lllegaldrone.userdb u
			LEFT JOIN 
				lllegaldrone.playdb p on p.userID = u.ID
			WHERE 
                u.Usergroup = ? 
            GROUP BY 
                u.CreateIDDate, u.ID, u.UserName, u.CreateAdminName
            ORDER BY 
                MIN(u.CreateIDDate) ', IF(in_asc, 'ASC', 'DESC'), '
            LIMIT ? OFFSET ?;
        ');
        PREPARE stmt FROM @sql;
        SET @userGroup = in_userGroup;
        SET @maxCnt = in_maxCnt;
        SET @offset = in_offset;
        EXECUTE stmt USING @userGroup, @maxCnt, @offset;
        DEALLOCATE PREPARE stmt;
    END IF;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL GetSearchUserInfo(0, 15, true, '권', true, 0, @return_state);
CALL GetSearchUserInfo(0, 15, true, '', false, 0,  @return_state);
SELECT @return_state AS return_state;

SELECT                
	ROW_NUMBER() OVER (ORDER BY MIN(u.CreateIDDate) ASC) AS idx,
	u.CreateIDDate, 
	u.ID, 
	u.UserName, 
    COALESCE(COUNT(p.UserID), 0) AS MissionResult_count, -- scenarioDB에서 CreateID 개수 세기
	COALESCE(MAX(p.PlayScenarioDateTime), '1999-01-01 00:00:00') AS PlayScenarioDateTime,
	u.CreateAdminName 
FROM 
	lllegaldrone.userdb u
LEFT JOIN 
	lllegaldrone.playdb p on p.userID = u.ID
WHERE 
	u.UserName LIKE "%%" and u.Usergroup = 1
GROUP BY 
	u.CreateIDDate, u.ID, u.UserName, u.CreateAdminName
ORDER BY 
	MIN(u.CreateIDDate) asc
LIMIT 15 OFFSET 0;