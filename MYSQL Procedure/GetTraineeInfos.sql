SELECT * FROM lllegaldrone.userdb;
SELECT * FROM lllegaldrone.playdb;

# 훈련생 정보들
# 프로시저 삭제
Drop Procedure GetTraineeInfos;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetTraineeInfos;

# 프로시저 생성
DELIMITER //

CREATE PROCEDURE GetTraineeInfos(
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
        SET return_state = 0;
    END;

    IF in_asc THEN
		WITH LatestPlay AS (
			SELECT 
				p.UserID, 
				MAX(p.PlayScenarioDateTime) AS LatestPlayDate
			FROM 
				lllegaldrone.playdb p
			GROUP BY 
				p.UserID
		),
		LatestPlayDetails AS (
			SELECT 
				p.UserID,
				p.PlayScenarioDateTime,
				p.ScenarioID,
				p.Missionresult,
				ROW_NUMBER() OVER (PARTITION BY p.UserID ORDER BY p.PlayScenarioDateTime DESC) AS rn
			FROM 
				lllegaldrone.playdb p
		)

		SELECT 
			ROW_NUMBER() OVER (ORDER BY u.CreateIDDate) AS idx,
			u.CreateIDDate, 
			u.ID, 
			u.UserName, 
			COALESCE(playCount.Count, 0) AS Scenariocount,
			lp.LatestPlayDate,
			s.PlayMode,
			lpd.Missionresult,
			CASE 
				WHEN lpd.PlayScenarioDateTime IS NULL THEN 'false'
				WHEN DATEDIFF(CURRENT_DATE, lpd.PlayScenarioDateTime) > 90 THEN 'false' 
				ELSE 'true' 
			END AS btnActive,
			u.CreateAdminName
		FROM 
			lllegaldrone.userdb u
		LEFT JOIN 
			LatestPlay lp ON lp.UserID = u.ID
		LEFT JOIN 
			LatestPlayDetails lpd ON lpd.UserID = u.ID AND lpd.rn = 1  -- 최신 데이터만 선택
		LEFT JOIN 
			lllegaldrone.scenariodb s ON s.ScenarioID = lpd.ScenarioID
		LEFT JOIN (
			SELECT 
				UserID, 
				COUNT(*) AS Count 
			FROM 
				lllegaldrone.playdb 
			GROUP BY 
				UserID
		) AS playCount ON playCount.UserID = u.ID
		WHERE 
			u.Usergroup = 2
			AND (u.UserName LIKE CONCAT('%', in_name, '%'))
		ORDER BY 
			idx asc  -- 올바른 정렬
		LIMIT in_maxCnt OFFSET in_offset;

    ELSE
				WITH LatestPlay AS (
			SELECT 
				p.UserID, 
				MAX(p.PlayScenarioDateTime) AS LatestPlayDate
			FROM 
				lllegaldrone.playdb p
			GROUP BY 
				p.UserID
		),
		LatestPlayDetails AS (
			SELECT 
				p.UserID,
				p.PlayScenarioDateTime,
				p.ScenarioID,
				p.Missionresult,
				ROW_NUMBER() OVER (PARTITION BY p.UserID ORDER BY p.PlayScenarioDateTime DESC) AS rn
			FROM 
				lllegaldrone.playdb p
		)

		SELECT 
			ROW_NUMBER() OVER (ORDER BY u.CreateIDDate) AS idx,
			u.CreateIDDate, 
			u.ID, 
			u.UserName, 
			COALESCE(playCount.Count, 0) AS Scenariocount,
			lp.LatestPlayDate,
			s.PlayMode,
			lpd.Missionresult,
			CASE 
				WHEN lpd.PlayScenarioDateTime IS NULL THEN 'false'
				WHEN DATEDIFF(CURRENT_DATE, lpd.PlayScenarioDateTime) > 90 THEN 'false' 
				ELSE 'true' 
			END AS btnActive,
			u.CreateAdminName
		FROM 
			lllegaldrone.userdb u
		LEFT JOIN 
			LatestPlay lp ON lp.UserID = u.ID
		LEFT JOIN 
			LatestPlayDetails lpd ON lpd.UserID = u.ID AND lpd.rn = 1  -- 최신 데이터만 선택
		LEFT JOIN 
			lllegaldrone.scenariodb s ON s.ScenarioID = lpd.ScenarioID
		LEFT JOIN (
			SELECT 
				UserID, 
				COUNT(*) AS Count 
			FROM 
				lllegaldrone.playdb 
			GROUP BY 
				UserID
		) AS playCount ON playCount.UserID = u.ID
		WHERE 
			u.Usergroup = 2
			AND (u.UserName LIKE CONCAT('%', in_name, '%'))
		ORDER BY 
			idx desc  -- 올바른 정렬
		LIMIT in_maxCnt OFFSET in_offset;
    END IF;

    SET return_state = 1;
END //

DELIMITER ;

# 프로시저 실행
CALL GetTraineeInfos(0,100,false,"",false,2,@return_state);
Select @return_msg;
Select @return_state;
Select @return_count;

CALL GetTraineeInfos(0, 5, True, '', False, 2, @return_state);

        
        
        


WITH LatestPlay AS (
    SELECT 
        p.UserID, 
        MAX(p.PlayScenarioDateTime) AS LatestPlayDate
    FROM 
        lllegaldrone.playdb p
    GROUP BY 
        p.UserID
),
LatestPlayDetails AS (
    SELECT 
        p.UserID,
        p.PlayScenarioDateTime,
        p.ScenarioID,
        p.Missionresult,
        ROW_NUMBER() OVER (PARTITION BY p.UserID ORDER BY p.PlayScenarioDateTime DESC) AS rn
    FROM 
        lllegaldrone.playdb p
)

SELECT 
    ROW_NUMBER() OVER (ORDER BY u.CreateIDDate) AS idx,
    u.CreateIDDate, 
    u.ID, 
    u.UserName, 
    u.Scenariocount, 
    lp.LatestPlayDate,
    s.PlayMode,
    lpd.Missionresult,
    CASE 
        WHEN lpd.PlayScenarioDateTime IS NULL THEN 'false'
        WHEN DATEDIFF(CURRENT_DATE, lpd.PlayScenarioDateTime) > 90 THEN 'false' 
        ELSE 'true' 
    END AS btnActive,
    u.CreateAdminName
FROM 
    lllegaldrone.userdb u
LEFT JOIN 
    LatestPlay lp ON lp.UserID = u.ID
LEFT JOIN 
    LatestPlayDetails lpd ON lpd.UserID = u.ID AND lpd.rn = 1  -- 최신 데이터만 선택
LEFT JOIN 
    lllegaldrone.scenariodb s ON s.ScenarioID = lpd.ScenarioID
WHERE 
    u.Usergroup = 2
    AND (u.UserName LIKE CONCAT('%', "", '%'))
ORDER BY 
    idx DESC  -- 올바른 정렬
LIMIT 100 OFFSET 0;









WITH LatestPlay AS (
    SELECT 
        p.UserID, 
        MAX(p.PlayScenarioDateTime) AS LatestPlayDate
    FROM 
        lllegaldrone.playdb p
    GROUP BY 
        p.UserID
),
LatestPlayDetails AS (
    SELECT 
        p.UserID,
        p.PlayScenarioDateTime,
        p.ScenarioID,
        p.Missionresult,
        ROW_NUMBER() OVER (PARTITION BY p.UserID ORDER BY p.PlayScenarioDateTime DESC) AS rn
    FROM 
        lllegaldrone.playdb p
)

SELECT 
    ROW_NUMBER() OVER (ORDER BY u.CreateIDDate) AS idx,
    u.CreateIDDate, 
    u.ID, 
    u.UserName, 
    COALESCE(playCount.Count, 0) AS Scenariocount,  -- 플레이 기록 개수
    lp.LatestPlayDate,
    s.PlayMode,
    lpd.Missionresult,
    CASE 
        WHEN lpd.PlayScenarioDateTime IS NULL THEN 'false'
        WHEN DATEDIFF(CURRENT_DATE, lpd.PlayScenarioDateTime) > 90 THEN 'false' 
        ELSE 'true' 
    END AS btnActive,
    u.CreateAdminName
FROM 
    lllegaldrone.userdb u
LEFT JOIN 
    LatestPlay lp ON lp.UserID = u.ID
LEFT JOIN 
    LatestPlayDetails lpd ON lpd.UserID = u.ID AND lpd.rn = 1  -- 최신 데이터만 선택
LEFT JOIN 
    lllegaldrone.scenariodb s ON s.ScenarioID = lpd.ScenarioID
LEFT JOIN (
    SELECT 
        UserID, 
        COUNT(*) AS Count 
    FROM 
        lllegaldrone.playdb 
    GROUP BY 
        UserID
) AS playCount ON playCount.UserID = u.ID
WHERE 
    u.Usergroup = 2
    AND (u.UserName LIKE CONCAT('%', "", '%'))
ORDER BY 
    idx DESC  -- 올바른 정렬
LIMIT 100 OFFSET 0;