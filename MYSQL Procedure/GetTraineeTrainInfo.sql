# 훈련생 훈련 정보 불러오기
# 프로시저 삭제
Drop Procedure GetTraineeTrainInfos;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetTraineeTrainInfos;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetTraineeTrainInfos(
    IN in_offset INT,
    IN in_maxCnt INT,
    IN in_asc BOOL,
    IN in_userid VARCHAR(255),
    OUT return_state INT
)
BEGIN
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

    -- 기본 쿼리
    SET @order := CASE WHEN in_asc THEN 'ASC' ELSE 'DESC' END;
                                                
	SET @sql := CONCAT('SELECT 
						rankedPlayers.idx, 
						rankedPlayers.PlayScenarioDateTime, 
						s.MissionMap, 
						s.Weather, 
						s.TimeZone, 
						s.PlayMode, 
						rankedPlayers.PlayScenarioTime, 
						s.limitPlaytime, 
						rankedPlayers.Missionresult, 
						CASE 
							WHEN u.Usergroup = 1 THEN -1 
							ELSE rankedPlayers.ranks 
						END AS ranks,
						CASE 
							WHEN rankedPlayers.PlayScenarioDateTime IS NULL THEN "false"
							WHEN DATEDIFF(CURRENT_DATE, rankedPlayers.PlayScenarioDateTime) > 90 THEN "false"
							ELSE "true"
						END AS btnActive,
                        s.CreateUser as Admin
					FROM (
						SELECT 
							p.idx, 
							p.PlayScenarioDateTime, 
							p.UserID, 
							p.KillCount, 
							p.ScenarioID, 
							p.TeamMissionresult as Missionresult,
							p.PlayScenarioTime,
							RANK() OVER (PARTITION BY p.ScenarioID ORDER BY p.KillCount DESC) AS ranks
						FROM lllegaldrone.playdb p    
					) AS rankedPlayers
					INNER JOIN lllegaldrone.scenariodb s ON rankedPlayers.ScenarioID = s.ScenarioID
					INNER JOIN lllegaldrone.userdb u ON rankedPlayers.UserID = u.ID
					WHERE rankedPlayers.UserID = ?
					ORDER BY rankedPlayers.PlayScenarioDateTime ', @order, ', rankedPlayers.ScenarioID, ranks
                    LIMIT ? OFFSET ?;');
                        
    -- Prepare statement
    PREPARE stmt FROM @sql;

    SET @UserID = in_userid;
    SET @maxCnt = in_maxCnt;
    SET @offset = in_offset;

    -- Execute statement
    EXECUTE stmt USING @UserID, @maxCnt, @offset;
    
    -- Cleanup
    DEALLOCATE PREPARE stmt;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL GetTraineeTrainInfos(0, 15, false, "test001", @return_state);
CALL GetTraineeTrainInfos(0, 15, True, '111111111', @return_state);
Select @return_state;

# 예시
SELECT rankedPlayers.idx, rankedPlayers.PlayScenarioDateTime, s.MissionMap, s.Weather, s.TimeZone, s.PlayMode, s.Playtime, s.limitPlaytime, rankedPlayers.Missionresult, rankedPlayers.ranks,
			CASE 
				WHEN rankedPlayers.PlayScenarioDateTime IS NULL THEN 'false'
				WHEN DATEDIFF(CURRENT_DATE, rankedPlayers.PlayScenarioDateTime) > 90 THEN 'false' 
				ELSE 'true' 
			END AS btnActive
FROM (
    SELECT p.idx, p.PlayScenarioDateTime, p.UserID, p.KillCount, p.ScenarioID, p.Missionresult,
           RANK() OVER (PARTITION BY p.ScenarioID ORDER BY p.KillCount DESC) AS ranks
    FROM lllegaldrone.playdb p    
) AS rankedPlayers
INNER JOIN lllegaldrone.scenariodb s ON rankedPlayers.ScenarioID = s.ScenarioID
WHERE rankedPlayers.UserID = "yoyo1234"
ORDER BY rankedPlayers.PlayScenarioDateTime asc, rankedPlayers.ScenarioID, rankedPlayers.ranks
LIMIT 15 OFFSET 0;


SELECT rankedPlayers.idx, rankedPlayers.PlayScenarioDateTime, s.MissionMap, s.Weather, s.TimeZone, s.PlayMode, rankedPlayers.PlayScenarioTime, s.limitPlaytime, rankedPlayers.Missionresult, rankedPlayers.ranks,
						CASE 
							WHEN rankedPlayers.PlayScenarioDateTime IS NULL THEN "false"
							WHEN DATEDIFF(CURRENT_DATE, rankedPlayers.PlayScenarioDateTime) > 90 THEN "false"
							ELSE "true"
						END AS btnActive
						FROM (
							SELECT p.idx, p.PlayScenarioDateTime, p.UserID, p.KillCount, p.ScenarioID, p.Missionresult, p.PlayScenarioTime,
								   RANK() OVER (PARTITION BY p.ScenarioID ORDER BY p.KillCount DESC) AS ranks
							FROM lllegaldrone.playdb p    
						) AS rankedPlayers
						INNER JOIN lllegaldrone.scenariodb s ON rankedPlayers.ScenarioID = s.ScenarioID
						WHERE rankedPlayers.UserID = "333333333"
						ORDER BY rankedPlayers.PlayScenarioDateTime asc, rankedPlayers.ScenarioID, rankedPlayers.ranks
						LIMIT 15 OFFSET 0;
                        
                        
                        
                        
                        
                        
                        
                        
                        
SELECT 
    rankedPlayers.idx, 
    rankedPlayers.PlayScenarioDateTime, 
    s.MissionMap, 
    s.Weather, 
    s.TimeZone, 
    s.PlayMode,     
    rankedPlayers.PlayScenarioTime, 
    s.limitPlaytime, 
    rankedPlayers.Missionresult,     
    CASE 
        WHEN u.Usergroup = 1 THEN -1 
        ELSE rankedPlayers.ranks 
    END AS ranks,
    CASE 
        WHEN rankedPlayers.PlayScenarioDateTime IS NULL THEN 'false'
        WHEN DATEDIFF(CURRENT_DATE, rankedPlayers.PlayScenarioDateTime) > 90 THEN 'false' 
        ELSE 'true' 
    END AS btnActive,
    s.CreateUser
FROM (
    SELECT 
        p.idx, 
        p.PlayScenarioDateTime, 
        p.UserID, 
        p.KillCount, 
        p.ScenarioID, 
        p.Missionresult,
        p.PlayScenarioTime,
        RANK() OVER (PARTITION BY p.ScenarioID ORDER BY p.KillCount DESC) AS ranks
    FROM lllegaldrone.playdb p    
) AS rankedPlayers
INNER JOIN lllegaldrone.scenariodb s ON rankedPlayers.ScenarioID = s.ScenarioID
INNER JOIN lllegaldrone.userdb u ON rankedPlayers.UserID = u.ID
WHERE rankedPlayers.UserID = "111111111"
ORDER BY rankedPlayers.PlayScenarioDateTime ASC, rankedPlayers.ScenarioID, ranks
LIMIT 15 OFFSET 0;