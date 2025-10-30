# 룬련 정보 불러오기
# 프로시저 삭제
Drop Procedure GetTrainingInfos;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetTrainingInfos;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetTrainingInfos(
    IN in_offset INT,
    IN in_maxCnt INT,
    IN in_asc BOOL,
    IN in_creator VARCHAR(255), -- 입력 파라미터 이름 수정
    IN in_issearch BOOL,
    OUT return_state INT
)
BEGIN
    DECLARE creator VARCHAR(255);

    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0; -- 에러 발생 시 상태값 설정
    END;

    IF in_issearch THEN
        SET creator = in_creator; -- 입력 파라미터 사용
    ELSE
        SET creator = ""; -- 기본값 설정
    END IF;

        SET @order_by = CASE WHEN in_asc THEN 'ASC' ELSE 'DESC' END; -- 정렬 순서 결정

		SET @sql = CONCAT(
			'SELECT DISTINCT 
				s.ScenarioID, 
				p.PlayScenarioDateTime, 
				s.MissionMap, 
				s.Weather, 
				s.TimeZone, 
				s.PlayMode, 
				p.PlayScenarioTime, 
				p.TeamMissionresult, 
				CASE 
					WHEN DATEDIFF(CURRENT_DATE, p.PlayScenarioDateTime) > 90 THEN ''false'' 
					ELSE ''true'' 
				END AS btnActive,
				s.CreateUser
			FROM 
				lllegaldrone.playdb p
			INNER JOIN 
				lllegaldrone.scenariodb s ON p.ScenarioID = s.ScenarioID 
			WHERE
				s.CreateUser LIKE CONCAT(''%'', ?, ''%'') 
			ORDER BY 
				s.ScenarioID ', @order_by, ' 
			LIMIT ? OFFSET ?'
		);

    PREPARE stmt FROM @sql;
    SET @creator = creator;
    SET @maxCnt = in_maxCnt;
    SET @offset = in_offset;

    EXECUTE stmt USING @creator, @maxCnt, @offset;
    DEALLOCATE PREPARE stmt;
    SET return_state = 1; -- 성공 상태값 설정
END //
DELIMITER ;

# 프로시저 실행
CALL GetTrainingInfos(0, 100, false, "", false, @return_state);
Select @return_state;

# 예제
SELECT DISTINCT 
    s.ScenarioID, 
    p.PlayScenarioDateTime, 
    s.MissionMap, 
    s.Weather, 
    s.TimeZone, 
    s.PlayMode, 
    p.PlayScenarioTime,
    p.TeamMissionresult, 
    CASE 
        WHEN DATEDIFF(CURRENT_DATE, p.PlayScenarioDateTime) > 90 THEN 'false' 
        ELSE 'true' 
    END AS btnActive,
    s.CreateUser
FROM 
    lllegaldrone.playdb p
INNER JOIN 
    lllegaldrone.scenariodb s ON p.ScenarioID = s.ScenarioID 
WHERE
	s.CreateUser LIKE "%%"
ORDER BY 
    s.ScenarioID ASC
LIMIT 100 OFFSET 0;
