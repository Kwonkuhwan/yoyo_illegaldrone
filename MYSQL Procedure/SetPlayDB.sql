# 새 시나리오 작성
# 프로시저 삭제
Drop Procedure SetPlayDB;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS SetPlayDB;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE SetPlayDB(
    IN in_scenarioID VARCHAR(255),
    IN in_PlayID VARCHAR(255),
    IN in_UserID VARCHAR(255),
    IN in_PlayScenarioDateTime DATETIME,
    In in_PlayTime TIME,
    In in_UserCount INT,
    In in_ScenarioPlayTime TIME,
    In in_KillCount VARCHAR(255),
    In in_MissionResult INT,
    in in_TeamMissionResult INT,
    OUT return_state INT
)
BEGIN
    -- 예외 발생 시 처리
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;  -- 실패 상태
    END;

    INSERT INTO `lllegaldrone`.`playdb`
	(
    `ScenarioID`,
	`PlayID`,
	`UserID`,
	`PlayScenarioDateTime`,
	`PlayScenarioTime`,
	`PlayUserCount`,
	`Scenarioplaytime`,
	`KillCount`,
	`Missionresult`,
	`TeamMissionresult`
    )
	VALUES
	(
    in_scenarioID,
	in_PlayID,
	in_UserID,
	in_PlayScenarioDateTime,
	in_PlayTime,				-- 훈련 시간
	in_UserCount,
	in_ScenarioPlayTime,		-- 훈련 제한 시간
	in_KillCount,
	in_MissionResult,
	in_TeamMissionResult);

    SET return_state = 1;  -- 성공 상태
END //

DELIMITER ;

INSERT INTO `lllegaldrone`.`playdb`
	(
    `ScenarioID`,
	`PlayID`,
	`UserID`,
	`PlayScenarioDateTime`,
	`PlayScenarioTime`,
	`PlayUserCount`,
	`Scenarioplaytime`,
	`KillCount`,
	`Missionresult`,
	`TeamMissionresult`
    )
	VALUES
	(
    CONCAT('scenario_', LPAD(FLOOR(RAND() * 1000), 4, '0')),  -- ScenarioID (예: scenario_0001)
    CONCAT('playID', LPAD(FLOOR(RAND() * 1000), 4, '0')),      -- PlayID (예: play_0001)
    CONCAT('user_', LPAD(FLOOR(RAND() * 100), 3, '0')),       -- UserID (예: user_001)
    NOW() - INTERVAL FLOOR(RAND() * 30) DAY,          -- PlayScenarioDateTime (최근 30일 이내)
    TIME(NOW() - INTERVAL FLOOR(RAND() * 3600) SECOND), -- PlayTime (현재 시간 기준)
    FLOOR(RAND() * 10) + 1,                             -- UserCount (1~10명)
    TIME(SEC_TO_TIME(FLOOR(RAND() * 3600))),            -- ScenarioPlayTime (최대 1시간)
    FLOOR(RAND() * 50),                                 -- KillCount (0~49)
    FLOOR(RAND() * 2),                                  -- MissionResult (0 또는 1)
    FLOOR(RAND() * 2));                                  -- TeamMissionResult (0 또는 1)

CALL SetPlayDB(
    CONCAT('scenario_', LPAD(FLOOR(RAND() * 1000), 4, '0')),  -- ScenarioID (예: scenario_0001)
    CONCAT('play_', LPAD(FLOOR(RAND() * 1000), 4, '0')),      -- PlayID (예: play_0001)
    CONCAT('user_', LPAD(FLOOR(RAND() * 100), 3, '0')),       -- UserID (예: user_001)
    NOW() - INTERVAL FLOOR(RAND() * 30) DAY,          -- PlayScenarioDateTime (최근 30일 이내)
    TIME(NOW() - INTERVAL FLOOR(RAND() * 3600) SECOND), -- PlayTime (현재 시간 기준)
    FLOOR(RAND() * 10) + 1,                             -- UserCount (1~10명)
    TIME(SEC_TO_TIME(FLOOR(RAND() * 3600))),            -- ScenarioPlayTime (최대 1시간)
    FLOOR(RAND() * 50),                                 -- KillCount (0~49)
    FLOOR(RAND() * 2),                                  -- MissionResult (0 또는 1)
    FLOOR(RAND() * 2),                                  -- TeamMissionResult (0 또는 1)
    @return_state                                       -- return_state (OUT 변수)
);

SELECT @return_state; -- return_state 값 확인

SELECT * FROM lllegaldrone.playdb;

CALL SetPlayDB('secn_0001', 12345678, 'yoyo5678', '2025-04-17 11:59:00', '13:23:57', 1, '17:00:00', '0', 0, 0,  @return_state);