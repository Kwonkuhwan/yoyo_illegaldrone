# 교관 정보 불러오기
# 프로시저 삭제
Drop Procedure GetInstructorInfo;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetInstructorInfo;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetInstructorInfo(
    IN in_id VARCHAR(255),
    Out return_state INT
)
BEGIN
	DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

	SELECT 
		u.UserName, 
		u.Gender, 
		u.Usergroup, 
		u.ID, 
		COUNT(CASE WHEN p.TeamMissionresult = 1 THEN 1 END) AS Totalcount, 
		COUNT(p.TeamMissionresult) AS MissionresultTotalcount,  -- 수정: NULL도 포함되도록 COUNT(p.Missionresult) 사용
		u.EndAccessDate
	FROM 
		lllegaldrone.userdb u
	LEFT JOIN 
		lllegaldrone.playdb p ON u.ID = p.UserID
	WHERE 
		u.ID = in_id
	GROUP BY 
		u.UserName, u.Gender, u.Usergroup, u.ID;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL GetInstructorInfo('SMW', @return_state);
Select @return_state;

SELECT 
    u.UserName, 
    u.Gender, 
    u.Usergroup, 
    u.ID, 
    COUNT(CASE WHEN p.Missionresult = 1 THEN 1 END) AS Totalcount, 
    COUNT(p.Missionresult) AS MissionresultTotalcount,  -- 수정: NULL도 포함되도록 COUNT(p.Missionresult) 사용
    u.EndAccessDate
FROM 
    lllegaldrone.userdb u
LEFT JOIN 
    lllegaldrone.playdb p ON u.ID = p.UserID
WHERE 
    u.ID = "yoyo1234"
GROUP BY 
    u.UserName, u.Gender, u.Usergroup, u.ID;
    
    