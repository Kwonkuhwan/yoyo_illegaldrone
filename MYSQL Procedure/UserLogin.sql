# 유저 로그인
# 프로시저 삭제
Drop Procedure UserLogin;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS UserLogin;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE UserLogin(
    IN in_id VARCHAR(255), 
    IN in_pw VARCHAR(255), 
    OUT return_msg VARCHAR(255), 
    OUT return_state BOOLEAN
)
BEGIN
    DECLARE user_count INT;
    DECLARE delete_date DATETIME;

    -- 사용자의 정보를 확인합니다.
    SELECT COUNT(*), MAX(DeleteIDDate) INTO user_count, delete_date
    FROM lllegaldrone.userdb
    WHERE ID = in_id;

    -- ID가 존재하는지 확인
    IF user_count = 0 THEN
        SET return_msg = '일치하는 계정이 없습니다.\n계정 정보를 확인하고 다시 로그인 해주세요.';
        SET return_state = 3;  -- 실패 상태
    ELSE
        -- DeleteDate 확인
        IF delete_date <= NOW() THEN
            SET return_msg = '이용이 제한 된 계정입니다.\n최고 관리자 또는 교관에게 문의 바랍니다.';
            SET return_state = 2;  -- 실패 상태
        ELSE
            -- 비밀번호 확인
            SELECT COUNT(*) INTO user_count
            FROM lllegaldrone.userdb
            WHERE ID = in_id AND PW = in_pw;

            IF user_count > 0 THEN
                SET return_msg = '로그인 성공';
                SET return_state = 0;  -- 성공 상태
                UPDATE `lllegaldrone`.`userdb`
				SET
				`EndAccessDate` = Now()
				WHERE `ID` = in_id;

            ELSE
                SET return_msg = '비밀번호가 잘못되었습니다. 확인 후 다시 로그인해주세요.';
                SET return_state = 1;  -- 실패 상태
            END IF;
        END IF;
    END IF;    
END //
DELIMITER ;

# 프로시저 실행
# 0 : 로그인 성공
# 1 : 비밀번호 오류
# 2 : 삭제 계정
# 3 : 아이디 없음
CALL UserLogin("123456789", "000000", @return_msg, @return_state);
SELECT @return_msg;  -- 결과 메시지를 확인합니다.
Select @return_state;

SELECT COUNT(*) as user_count, DeleteIDDate as deleteDate
FROM lllegaldrone.userdb
WHERE ID = "123456789";

UPDATE `lllegaldrone`.`userdb`
SET
`EndAccessDate` = Now()
WHERE `ID` = "yoyo1234";
