﻿using HarmonyLib;
using JetBrains.Annotations;
using Kingmaker;
using Kingmaker.Blueprints.JsonSystem;
using System;
using TabletopTweaks;
using TabletopTweaks.Config;
using TabletopTweaks.Utilities;
using UnityModManagerNet;
using TabletopTweaks.Extensions;


namespace WOTR_WoljifRomanceMod
{
    [HarmonyPatch(typeof(BlueprintsCache), "Init")]
    class debugmenu
    {
        static void Postfix()
        {
            DialogTools.NewDialogs.LoadDialogIntoGame("enGB");

            createDebugMenu();
            createSimpleConditionalCue();
            createComplexConditionalCue();
            createConditionalAnswers();
            createSkillChecks();
            createActionTest();
            createSimpleCutscene();
            //Complex cutscene handled in areawatcher
        }



        static public void createDebugMenu()
        {
            var originalanswers = Resources.GetBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList>("e41585da330233143b34ef64d7d62d69");
            var starttestcue = DialogTools.CreateCue("TEST_cw_starttesting");
            var endtestcue = DialogTools.CreateCue("TEST_cw_donetesting");
            var starttestanswer = DialogTools.CreateAnswer("TEST_a_helpmetest");
            var endtestanswer = DialogTools.CreateAnswer("TEST_a_donetesting");
            var debuganswerlist = DialogTools.CreateAnswersList("TEST_L_debugmenu");
            DialogTools.ListAddAnswer(originalanswers, starttestanswer, 12);
            DialogTools.AnswerAddNextCue(starttestanswer, starttestcue);
            DialogTools.AnswerAddNextCue(endtestanswer, endtestcue);
            DialogTools.ListAddAnswer(debuganswerlist, endtestanswer);
            DialogTools.CueAddAnswersList(starttestcue, debuganswerlist);
            DialogTools.CueAddAnswersList(endtestcue, originalanswers);
        }
        static public void createSimpleConditionalCue()
        {
            var debuganswerlist = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList>("TEST_L_debugmenu");

            var simpleconditionalanswer = DialogTools.CreateAnswer("TEST_a_conditionalcue");
            var simpleconditionalcuetrue = DialogTools.CreateCue("TEST_cw_trueconditionalcue");
            var simpleconditionalcuefalse = DialogTools.CreateCue("TEST_cw_falseconditionalcue");
            var simplecondition = ConditionalTools.CreateCondition<Kingmaker.Designers.EventConditionActionSystem.Conditions.PcRace>("isplayertiefling", cond =>
            {
                cond.Race = Kingmaker.Blueprints.Race.Tiefling;
            });
            DialogTools.CueAddCondition(simpleconditionalcuetrue, simplecondition);
            DialogTools.AnswerAddNextCue(simpleconditionalanswer, simpleconditionalcuetrue);
            DialogTools.AnswerAddNextCue(simpleconditionalanswer, simpleconditionalcuefalse);
            DialogTools.ListAddAnswer(debuganswerlist, simpleconditionalanswer, 0);
            DialogTools.CueAddAnswersList(simpleconditionalcuetrue, debuganswerlist);
            DialogTools.CueAddAnswersList(simpleconditionalcuefalse, debuganswerlist);
        }
        static public void createComplexConditionalCue()
        {
            var debuganswerlist = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList>("TEST_L_debugmenu");

            var complexconditionalanswer = DialogTools.CreateAnswer("TEST_a_complexconditionalcue");
            var complexconditionalcuetrue = DialogTools.CreateCue("TEST_cw_truecomplexconditionalcue");
            var complexconditionalcuefalse = DialogTools.CreateCue("TEST_cw_falsecomplexconditionalcue");
            // Build logic tree
            var complexlogic = ConditionalTools.CreateChecker();
            ConditionalTools.CheckerAddCondition(complexlogic,
                ConditionalTools.CreateCondition<Kingmaker.Designers.EventConditionActionSystem.Conditions.PcFemale>("isplayerfemale"));
            ConditionalTools.CheckerAddCondition(complexlogic,
                ConditionalTools.CreateLogicCondition("aasimarortiefling", Kingmaker.ElementsSystem.Operation.Or,
                    ConditionalTools.CreateCondition<Kingmaker.Designers.EventConditionActionSystem.Conditions.PcRace>("isplayertiefling",
                        bp => { bp.Race = Kingmaker.Blueprints.Race.Tiefling; }),
                    ConditionalTools.CreateCondition<Kingmaker.Designers.EventConditionActionSystem.Conditions.PcRace>("isplayeraasimar",
                        bp => { bp.Race = Kingmaker.Blueprints.Race.Aasimar; })));
            DialogTools.AnswerAddNextCue(complexconditionalanswer, complexconditionalcuetrue);
            DialogTools.AnswerAddNextCue(complexconditionalanswer, complexconditionalcuefalse);
            DialogTools.CueAddAnswersList(complexconditionalcuefalse, debuganswerlist);
            DialogTools.CueAddAnswersList(complexconditionalcuetrue, debuganswerlist);
            DialogTools.ListAddAnswer(debuganswerlist, complexconditionalanswer, 1);
        }

        static public void createConditionalAnswers()
        {
            var debuganswerlist = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList>("TEST_L_debugmenu");

            var genericcue = DialogTools.CreateCue("TEST_cw_generic");
            DialogTools.CueAddAnswersList(genericcue, debuganswerlist);
            var showcondanswertrue = DialogTools.CreateAnswer("TEST_a_trueconditionalanswer");
            var showcondanswerfalse = DialogTools.CreateAnswer("TEST_a_falseconditionalanswer");
            DialogTools.AnswerAddNextCue(showcondanswertrue, genericcue);
            DialogTools.AnswerAddNextCue(showcondanswerfalse, genericcue);
            DialogTools.AnswerAddShowCondition(showcondanswertrue,
                ConditionalTools.CreateCondition<Kingmaker.Designers.EventConditionActionSystem.Conditions.PcRace>("isplayertiefling",
                        bp => { bp.Race = Kingmaker.Blueprints.Race.Tiefling; }));
            DialogTools.AnswerAddShowCondition(showcondanswerfalse,
                ConditionalTools.CreateCondition<Kingmaker.Designers.EventConditionActionSystem.Conditions.PcRace>("isplayeraasimar",
                        bp => { bp.Race = Kingmaker.Blueprints.Race.Aasimar; }));
            DialogTools.ListAddAnswer(debuganswerlist, showcondanswerfalse, 2);
            DialogTools.ListAddAnswer(debuganswerlist, showcondanswertrue, 3);
            var unpickableanswer = DialogTools.CreateAnswer("TEST_a_unchoosableconditionalanswer");
            DialogTools.AnswerAddNextCue(unpickableanswer, genericcue);
            DialogTools.AnswerAddSelectCondition(unpickableanswer,
                ConditionalTools.CreateCondition<Kingmaker.Designers.EventConditionActionSystem.Conditions.PcRace>("isplayerelf",
                        bp => { bp.Race = Kingmaker.Blueprints.Race.Elf; }));
            DialogTools.ListAddAnswer(debuganswerlist, unpickableanswer, 4);
        }

        static public void createSkillChecks()
        {
            var debuganswerlist = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList>("TEST_L_debugmenu");

            var easycheckanswer = DialogTools.CreateAnswer("TEST_a_skillcheckeasy");
            var hardcheckanswer = DialogTools.CreateAnswer("TEST_a_skillcheckhard");
            var failedcheckcue = DialogTools.CreateCue("TEST_sf_failedskillcheck");
            var passedcheckcue = DialogTools.CreateCue("TEST_sp_passedskillcheck");
            DialogTools.CueAddAnswersList(failedcheckcue, debuganswerlist);
            DialogTools.CueAddAnswersList(passedcheckcue, debuganswerlist);
            var easycheck = DialogTools.CreateCheck("easycheck", Kingmaker.EntitySystem.Stats.StatType.CheckDiplomacy, 3, passedcheckcue, failedcheckcue);
            var hardcheck = DialogTools.CreateCheck("hardcheck", Kingmaker.EntitySystem.Stats.StatType.SkillAthletics, 30, passedcheckcue, failedcheckcue);
            DialogTools.AnswerAddNextCue(easycheckanswer, easycheck);
            DialogTools.AnswerAddNextCue(hardcheckanswer, hardcheck);
            DialogTools.ListAddAnswer(debuganswerlist, easycheckanswer, 5);
            DialogTools.ListAddAnswer(debuganswerlist, hardcheckanswer, 6);
        }

        static public void createActionTest()
        {
            var debuganswerlist = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList>("TEST_L_debugmenu");

            var actionanswer = DialogTools.CreateAnswer("TEST_a_action");
            var actioncue = DialogTools.CreateCue("TEST_cw_action");
            DialogTools.AnswerAddNextCue(actionanswer, actioncue);
            DialogTools.CueAddAnswersList(actioncue, debuganswerlist);
            var testaction = ActionTools.GenericAction<Kingmaker.Designers.EventConditionActionSystem.Actions.PlayCustomMusic>(bp =>
            {
                bp.MusicEventStart = "MUS_MysteryTheme_Play";
                bp.MusicEventStop = "MUS_MysteryTheme_Stop";
            });
            var stoptestaction = ActionTools.GenericAction<Kingmaker.Designers.EventConditionActionSystem.Actions.StopCustomMusic>();
            DialogTools.CueAddOnShowAction(actioncue, testaction);
            DialogTools.CueAddOnStopAction(actioncue, stoptestaction);
            DialogTools.ListAddAnswer(debuganswerlist, actionanswer, 7);
        }

        static public void createSimpleCutscene()
        {
            var debuganswerlist = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList>("TEST_L_debugmenu");

            var cutsceneanswer = DialogTools.CreateAnswer("TEST_a_cutscene");
            DialogTools.ListAddAnswer(debuganswerlist, cutsceneanswer, 8);
            var cutscenecue = DialogTools.CreateCue("TEST_cw_cutscene");
            DialogTools.AnswerAddNextCue(cutsceneanswer, cutscenecue);

            var newcue = DialogTools.CreateCue("TEST_cw_newdialog");
            var newdialog = DialogTools.CreateDialog("brandnewdialog", newcue);
            var DialogCommand = CommandTools.StartDialogCommand(newdialog, Companions.Woljif);
            //Cutscene
            // Track 1
            // Action: Lock Controls
            // Endgate: empty gate
            // Track 2
            // Action: Delay
            // Endgate: BarkGate
            // BarkGateTrack
            //Action: Bark
            //Endgate: DialogGate
            // DialogGateTrack
            // Action: Dialog start
            // Endgate: empty gate again
            var LockCommand = CommandTools.LockControlCommand();
            var emptyGate = CutsceneTools.CreateGate("emptygate");
            var Track1 = CutsceneTools.CreateTrack(emptyGate, LockCommand);

            var delayCommand = CommandTools.DelayCommand(1.0f);
            var barkcommand = CommandTools.BarkCommand("TEST_bark", Companions.Woljif);
            var dialogGateTrack = CutsceneTools.CreateTrack(emptyGate, DialogCommand);
            var dialoggate = CutsceneTools.CreateGate("dialoggate", dialogGateTrack);
            var BarkGateTrack = CutsceneTools.CreateTrack(dialoggate, barkcommand);
            var BarkGate = CutsceneTools.CreateGate("barkgate", BarkGateTrack);
            var Track2 = CutsceneTools.CreateTrack(BarkGate, delayCommand);

            Kingmaker.AreaLogic.Cutscenes.Track[] trackarray = { Track1, Track2 };
            var customcutscene = CutsceneTools.CreateCutscene("testcustomcutscene", false, trackarray);
            var playcutsceneaction = ActionTools.GenericAction<Kingmaker.Designers.EventConditionActionSystem.Actions.PlayCutscene>(bp =>
            {
                bp.m_Cutscene = Kingmaker.Blueprints.BlueprintReferenceEx.ToReference<Kingmaker.Blueprints.CutsceneReference>(customcutscene);
                bp.Owner = cutscenecue;
                bp.Parameters = new Kingmaker.Designers.EventConditionActionSystem.NamedParameters.ParametrizedContextSetter();
            });
            DialogTools.CueAddOnStopAction(cutscenecue, playcutsceneaction);
        }

        static public void createComplexCutscene(Kingmaker.Blueprints.EntityReference locator1, Kingmaker.Blueprints.EntityReference locator2)
        {
            // TEST CUTSCENE CREATION
            var debuganswerlist = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList>("TEST_L_debugmenu");
            var newdialog = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintDialog>("brandnewdialog");
            var cutsceneanswer2 = DialogTools.CreateAnswer("TEST_a_cutscene2");
            DialogTools.ListAddAnswer(debuganswerlist, cutsceneanswer2, 9);
            var cutscenecue2 = DialogTools.CreateCue("TEST_cw_cutscene2");
            DialogTools.AnswerAddNextCue(cutsceneanswer2, cutscenecue2);

            //cutscene
            //  trackA
            //    Command: Lockcontrol
            //    Endgate: DialogGate
            //        DialogGateTrack
            //          Command: start dialog
            //          Endgate: null
            //  trackB
            //    Command: move1, move2
            //    Endgate: cameragate
            //      CameraGateTrack
            //        Command: camerafollow
            //        Endgate: dialoggate

            // Create track A
            var lockcommand = CommandTools.LockControlCommand();
            var startdialogcommand = CommandTools.StartDialogCommand(newdialog, Companions.Woljif);
            var dialogGateTrack = CutsceneTools.CreateTrack(null, startdialogcommand);
            var dialogGate = CutsceneTools.CreateGate("dialoggatecomp", dialogGateTrack);
            var TrackA = CutsceneTools.CreateTrack(dialogGate, lockcommand);
            // Create Track B
            var cameracommand = CommandTools.CamFollowCommand(Companions.Woljif);
            var cameraGateTrack = CutsceneTools.CreateTrack(dialogGate, cameracommand);
            var cameragate = CutsceneTools.CreateGate("cameragate", cameraGateTrack);
            // Track B commands
            var unhideWoljifAction = ActionTools.HideUnitAction(Companions.Woljif, true);
            var moveWoljifAction = ActionTools.TranslocateAction(Companions.Woljif, locator1, true);
            Kingmaker.ElementsSystem.GameAction[] actionlist1 = { unhideWoljifAction, moveWoljifAction };
            var moveWoljifcommand = CommandTools.ActionCommand("movewoljifcommand", actionlist1);
            unhideWoljifAction.Owner = moveWoljifcommand;
            moveWoljifAction.Owner = moveWoljifcommand;
            var moveplayercommand = CommandTools.TranslocateCommand("moveplayercommand", Companions.Player, locator2, true);
            // Track B itself
            Kingmaker.AreaLogic.Cutscenes.CommandBase[] trackbcommands = { moveWoljifcommand, moveplayercommand };
            var TrackB = CutsceneTools.CreateTrack(cameragate, trackbcommands);
            // make the cutscene
            Kingmaker.AreaLogic.Cutscenes.Track[] cutscenetracks = { TrackA, TrackB };
            var customcutscene = CutsceneTools.CreateCutscene("testcomplexcutscene", false, cutscenetracks);
            var playcutsceneaction = ActionTools.PlayCutsceneAction(customcutscene, cutscenecue2);
            DialogTools.CueAddOnStopAction(cutscenecue2, playcutsceneaction);
        }

        static public void createAlternateCutscene(Kingmaker.Blueprints.EntityReference locator1, Kingmaker.Blueprints.EntityReference locator2, Kingmaker.Blueprints.EntityReference locator3)
        {
            // Dialog Bits
            var debuganswerlist = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList>("TEST_L_debugmenu");
            var newdialog = Resources.GetModBlueprint<Kingmaker.DialogSystem.Blueprints.BlueprintDialog>("brandnewdialog");
            var cutsceneanswer2 = DialogTools.CreateAnswer("TEST_a_cutscene2");
            DialogTools.ListAddAnswer(debuganswerlist, cutsceneanswer2, 9);
            var cutscenecue2 = DialogTools.CreateCue("TEST_cw_cutscene2");
            DialogTools.AnswerAddNextCue(cutsceneanswer2, cutscenecue2);

            /* STRUCTURE
             * Cutscene
             *   Track 0A
             *     Commands: [LockControl]
             *     EndGate: Gate 3
             *   Track 0B
             *     Commands: [Fadeout]
             *     Endgate: Gate 1
             *   Track 0C
             *     Commands: [Delay, Action(Transport Player, Transport Woljif), Delay]
             *     Endgate: Gate 1
             *   Track 0D
             *     Commands: [Camerafollowplayer]
             *     Endgate: Gate 1
             *     
             *   Gate 1
             *     Track 1A
             *       Commands: [Move Woljif, delay]
             *       Endgate: Gate 2
             *     Track 1B
             *       Commands: [Camera Follow]
             *       Endgate: Gate 3
             *   
             *   Gate 2
             *     Track 2A
             *       Commands: [Turn Woljif to face player, Bark]
             *       Endgate: Gate 3
             *   
             *   Gate 3
             *     Track 3A
             *       Commands: [Start Dialog]
             *       Endgate: Null
             */

            // Build 3rd section
            var startdialog_3A = CommandTools.StartDialogCommand(newdialog, Companions.Woljif);
            var Track_3A = CutsceneTools.CreateTrack(null, startdialog_3A);
            var Gate_3 = CutsceneTools.CreateGate("CutTestGate3", Track_3A);

            // Build 2nd Section
            var bark_2A = CommandTools.BarkCommand("CutTestBark2A", "TEST_bark2", Companions.Woljif);
            var turn_2A = CommandTools.LookAtCommand("CutTestTurn2A", Companions.Woljif, Companions.Player);
            Kingmaker.AreaLogic.Cutscenes.CommandBase[] commands_2A = { turn_2A, bark_2A };
            var Track_2A = CutsceneTools.CreateTrack(Gate_3, commands_2A);
            var Gate_2 = CutsceneTools.CreateGate("CutTestGate2", Track_2A);

            // Build 1st section
            var move_1A = CommandTools.WalkCommand("CutTestWalk1A", Companions.Woljif, locator3);
            var delay_1A = CommandTools.DelayCommand(0.5f);
            Kingmaker.AreaLogic.Cutscenes.CommandBase[] commands_1A = { move_1A, delay_1A };
            var track_1A = CutsceneTools.CreateTrack(Gate_2, commands_1A);
            var camera_1B = CommandTools.CamFollowCommand(Companions.Woljif);
            var track_1B = CutsceneTools.CreateTrack(Gate_3, camera_1B);
            Kingmaker.AreaLogic.Cutscenes.Track[] tracks_1 = { track_1A, track_1B };
            var Gate_1 = CutsceneTools.CreateGate("CutTestGate1", tracks_1);

            // Build 0th section
            var lock_0A = CommandTools.LockControlCommand();
            var track_0A = CutsceneTools.CreateTrack(Gate_3, lock_0A);
            var fade_0B = CommandTools.FadeoutCommand();
            var track_0B = CutsceneTools.CreateTrack(Gate_1, fade_0B);
            var delay1_0C = CommandTools.DelayCommand(0.5f);
            var movepc_0c = ActionTools.TranslocateAction(Companions.Player, locator1);
            var movewj_0c = ActionTools.TranslocateAction(Companions.Woljif, locator2);
            Kingmaker.ElementsSystem.GameAction[] actions_0c = { movepc_0c, movewj_0c };
            var move_0c = CommandTools.ActionCommand("CutTestTeleport", actions_0c);
            var delay2_0C = CommandTools.DelayCommand();
            Kingmaker.AreaLogic.Cutscenes.CommandBase[] commands_0c = { delay1_0C, move_0c, delay2_0C };
            var track_0c = CutsceneTools.CreateTrack(Gate_1, commands_0c);
            var camera_0d = CommandTools.CamFollowCommand(Companions.Player);
            var track_0d = CutsceneTools.CreateTrack(Gate_1, camera_0d);

            // Build cutscene
            Kingmaker.AreaLogic.Cutscenes.Track[] tracks0 = { track_0A, track_0B, track_0c, track_0d };
            var complexcutscene = CutsceneTools.CreateCutscene("CutTestScene", false, tracks0);

            // Attach to dialog
            var playcutsceneaction = ActionTools.GenericAction<Kingmaker.Designers.EventConditionActionSystem.Actions.PlayCutscene>(bp =>
            {
                bp.m_Cutscene = Kingmaker.Blueprints.BlueprintReferenceEx.ToReference<Kingmaker.Blueprints.CutsceneReference>(complexcutscene);
                bp.Owner = cutscenecue2;
                bp.Parameters = new Kingmaker.Designers.EventConditionActionSystem.NamedParameters.ParametrizedContextSetter();
            });
            DialogTools.CueAddOnStopAction(cutscenecue2, playcutsceneaction);
        }
    }
}