
using System.Collections.Generic;

/// <summary>
/// 热更Const
/// </summary>
public static partial class Const
{
    internal const long DefaultVibrateDuration = 50;//安卓手机震动强度
    internal static readonly float SHOW_CLOSE_INTERVAL = 1f;//出现关闭按钮的延迟

    public static readonly string HORIZONTAL = "Horizontal";
    public static readonly bool RepeatLevel = false;//是否循环关卡

    internal static class UserData
    {
        internal static readonly string MONEY                   = "UserData.MONEY";
        internal static readonly string GUIDE_ON                = "UserData.GUIDE_ON";

        internal static readonly string SHOW_RATING_COUNT       = "UserData.SHOW_RATING_COUNT";
        internal static readonly string GAME_LEVEL              = "UserData.GAME_LEVEL";
        internal static readonly string CAR_SKIN_ID             = "UserData.CAR_SKIN_ID";

        internal static readonly string USER_SPAWN_POINT_TYPE   = "UserData.USER_SPAWN_POINT_TYPE";

        internal static readonly string IS_WIN                  = "UserData.IS_WIN";
        internal static readonly string LEVEL_USED_TIME         = "UserData.LEVEL_USED_TIME";
        internal static readonly string LEVEL_FILL_NUM          = "UserData.LEVEL_FILL_NUM";
        internal static readonly string NMN_LEVEL_ID            = "UserData.NMN_LEVEL_ID";
        internal static readonly string FREE_AD_TIMES           = "UserData.FREE_AD_TIMES";
        internal static readonly string NO_ADS_BUYED            = "UserData.NO_ADS_BUYED";
    }
    
    internal static class MainMenuProcedureData
    {
        internal static readonly string CUR_BIG_LEVEL           = "MainMenuProcedureData.CUR_BIG_LEVEL";
        internal static readonly string CUR_LIT_LEVEL           = "MainMenuProcedureData.CUR_LIT_LEVEL";
        internal static readonly string FROM_GAME_OVER_BACK     = "MainMenuProcedureData.FROM_GAME_OVER_BACK";
        internal static readonly string BIG_LEVEL_STR           = "MainMenuProcedureData.BIG_LEVEL_STR";
        internal static readonly string LIT_LEVEL_STR           = "MainMenuProcedureData.LIT_LEVEL_STR";
    }
    internal static class LoadingLevelProcedureData
    {
        internal static readonly string LEVEL_ID                = "ChooseLevelProcedureData.LEVEL_ID";
        internal static readonly string LEVEL_READY_CALLBACK    = "ChooseLevelProcedureData.LEVEL_READY_CALLBACK";
        internal static readonly string SUDOKU_BOARD_UI_ID      = "ChooseLevelProcedureData.SUDOKU_BOARD_UI_ID";
    }

    internal static class GameOverProcedureData
    {
        internal static readonly string BACK_TO_MAIN_MENU       = "GameOverProcedureData.BACK_TO_MAIN_MENU";
    }

    internal static class SudokuPawnEntryData
    {
        internal static readonly string BOARD_PAWN_ENUM_NUM         = "SudokuPawnEntryData.BOARD_PAWN_ENUM_NUM";
        internal static readonly string BOARD_SIZE                  = "SudokuPawnEntryData.BOARD_SIZE";
        internal static readonly string BOARD_LIST                  = "SudokuPawnEntryData.BOARD_LIST";
        internal static readonly string ANSWER_LIST                 = "SudokuPawnEntryData.ANSWER_LIST";
        internal static readonly string BOARD_ON_READY_CALLBACK     = "SudokuPawnEntryData.BOARD_ON_READY_CALLBACK";
        internal static readonly string LOADING_PROGRESS_CALLBACK   = "SudokuPawnEntryData.LOADING_PROGRESS_CALLBACK";
        internal static readonly string USED_TIME                   = "SudokuPawnEntryData.USED_TIME";
    }

    internal static class GameRule
    {
        internal static readonly string NEED_RANDOM_PLAY    = "GameRule.NEED_RANDOM_PLAY";
        internal static readonly string DIFFICULTY_ID       = "GameRule.DIFFICULTY_ID";
        internal static readonly string BLACK_PAWN_NUM       = "GameRule.BLACK_PAWN_NUM";
    }
}
