using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

using LitJson;
using Assets.Scripts.UI;

public class BP_SaveLoadManager : MonoBehaviour, IButton
{
    public bool SaveOrLoad;
    public string fileName;

    //  Load시 instantiate를 위한 prefab들을 저장
    [SerializeField]
    private BP_Gear m_gearPrefab;
    [SerializeField]
    private BP_Shaft m_shaftPrefab;
    [SerializeField]
    private BP_Link m_linkPrefab;
    [SerializeField]
    private BP_Joint m_jointPrefab;
    [SerializeField]
    private BP_SlottedBar m_SlottedBarPrefab;

    public BlueprintManager m_blueprintManagerInstance
    {
        get
        {
            if (FindObjectOfType<BlueprintManager>())
                return FindObjectOfType<BlueprintManager>();
            else
            {
                print("BP_SaveLoadManager: Can't find BlueprintManager");
                return null;
            }
        }
    }

    #region blueprint object를 파일화하는데 필요한 정보들을 담는 클래스들
    [Serializable]
    public class BP_ObjectInfo
    {
        //  LitJson은 float형을 지원하지 않는다. 따라서 저장하는 데이터들을 double로 바꿔 저장한다.
        [Serializable]
        public struct doubleTransform
        {
            public double[] dPosition;
            public double[] dRotation;
            public double[] dScale;
            public doubleTransform(Transform fTransform)
            {
                Vector3 fPosition = fTransform.position;
                Quaternion fRotation = fTransform.rotation;
                Vector3 fScale = fTransform.localScale;

                dPosition = new double[3];
                dPosition[0] = fPosition.x; dPosition[1] = fPosition.y; dPosition[2] = fPosition.z;
                dRotation = new double[4];
                dRotation[0] = fRotation.x; dRotation[1] = fRotation.y;
                dRotation[2] = fRotation.z; dRotation[3] = fRotation.w;
                dScale = new double[3];
                dScale[0] = fScale.x; dScale[1] = fScale.y; dScale[2] = fScale.z;
            }

            public Vector3 getFloatPos()
            {
                return new Vector3((float)dPosition[0],
                    (float)dPosition[1],
                    (float)dPosition[2]);
            }
            public Quaternion getFloatRot()
            {
                return new Quaternion((float)dRotation[0],
                    (float)dRotation[1],
                    (float)dRotation[2],
                    (float)dRotation[3]);
            }
            public Vector3 getFloatScale()
            {
                return new Vector3((float)dScale[0],
                    (float)dScale[1],
                    (float)dScale[2]);
            }
        }
        //  오브젝트 정보들을 파일화 하기 위한 최소 정보
        public doubleTransform transform;
        public int instanceID;
        public BP_Object.type type;
        public uint parentBPIndex;

        public BP_ObjectInfo(Transform fTransform, int _id, BP_Object.type _type, uint _parentBPIndex)
        {
            transform = new doubleTransform(fTransform);
            instanceID = _id;
            type = _type;
            parentBPIndex = _parentBPIndex;
        }
    }

    [Serializable]
    public class BP_GearInfo : BP_ObjectInfo
    {
        public BP_GearInfo(Transform _transform, int _id, BP_Object.type _type, uint _parentBPIndex)
            : base(_transform, _id, _type, _parentBPIndex)
        { }
        public int[] childJointIDList;
        public int parentObjID;
    }

    [Serializable]
    private class BP_JointInfo : BP_ObjectInfo
    {
        public BP_JointInfo(Transform _transform, int _id, BP_Object.type _type, uint _parentBPIndex)
            : base(_transform, _id, _type, _parentBPIndex)
        { }
        public BP_Joint.JointType jointType;
        public int parentObjID;
        public int attachedObjID;
    }

    [Serializable]
    private class BP_LinkInfo : BP_ObjectInfo
    {
        public BP_LinkInfo(Transform _transform, int _id, BP_Object.type _type, uint _parentBPIndex)
            : base(_transform, _id, _type, _parentBPIndex)
        { }
        public int[] childJointIDList;
        public int startJointID, endJointID;
    }

    [Serializable]
    private class BP_SlottedBarInfo : BP_ObjectInfo
    {
        public BP_SlottedBarInfo(Transform _transform, int _id, BP_Object.type _type, uint _parentBPIndex)
            : base(_transform, _id, _type, _parentBPIndex)
        { }
        public int[] childJointIDList;
        public int startJointID, endJointID;
    }

    [Serializable]
    private class BP_ShaftInfo : BP_ObjectInfo
    {
        public BP_ShaftInfo(Transform _transform, int _id, BP_Object.type _type, uint _parentBPIndex)
            : base(_transform, _id, _type, _parentBPIndex)
        { }
        public BP_Shaft.ShaftType shaftType;
        public int[] childObjectIDList;
    }

    //  최종적으로 Json으로 저장될 Info들의 List를 모아놓은 InfoList
    [Serializable]
    private class BP_InfoList
    {
        public List<BP_GearInfo> gearInfoList = new List<BP_GearInfo>();
        public List<BP_ShaftInfo> shaftInfoList = new List<BP_ShaftInfo>();
        public List<BP_LinkInfo> linkInfoList = new List<BP_LinkInfo>();
        public List<BP_JointInfo> jointInfoList = new List<BP_JointInfo>();
        public List<BP_SlottedBarInfo> slottedBarInfoList = new List<BP_SlottedBarInfo>();

        public List<BP_ObjectInfo> getAllObjectInfos()
        {
            List<BP_ObjectInfo> objInfos = new List<BP_ObjectInfo>();
            foreach (BP_ObjectInfo gInfo in gearInfoList)
                objInfos.Add(gInfo);
            foreach (BP_ObjectInfo sInfo in shaftInfoList)
                objInfos.Add(sInfo);
            foreach (BP_ObjectInfo lInfo in linkInfoList)
                objInfos.Add(lInfo);
            foreach (BP_ObjectInfo jInfo in jointInfoList)
                objInfos.Add(jInfo);
            foreach (BP_ObjectInfo slInfo in slottedBarInfoList)
                objInfos.Add(slInfo);

            return objInfos;
        }

        public void printAllInfoLists()
        {
            int count = 0;
            foreach (BP_GearInfo gearInfo in this.gearInfoList)
            {
                print("GearInfo " + count++);
                print("position: " + gearInfo.transform.dPosition[0]);
                print("rotation: " + gearInfo.transform.dRotation[0]);
                print("scale: " + gearInfo.transform.dScale[0]);
                print("ID: " + gearInfo.instanceID);
                print("parent Blueprint index: " + gearInfo.parentBPIndex);
                if (gearInfo.parentObjID != 0)
                    print("parent Object ID: " + gearInfo.parentObjID);
            }

            count = 0;
            foreach (BP_ShaftInfo shaftInfo in this.shaftInfoList)
            {
                print("ShaftInfo " + count++);
                print("position: " + shaftInfo.transform.dPosition[0]);
                print("rotation: " + shaftInfo.transform.dRotation[0]);
                print("scale: " + shaftInfo.transform.dScale[0]);
                print("ID: " + shaftInfo.instanceID);
                print("parent Blueprint index: " + shaftInfo.parentBPIndex);
                print("shaft type: " + shaftInfo.shaftType);
                string childList = "child : ";
                foreach (int index in shaftInfo.childObjectIDList)
                {
                    childList += index.ToString() + ", ";
                }
                print(childList);
            }
        }
    }
    #endregion

    public void saveAll()
    {
        //  InfoList 생성
        BP_InfoList infoList = new BP_InfoList();
        //  모든 Blueprint object(BP_Object)를 탐색하여
        List<BP_Object> objs = new List<BP_Object>();
        foreach (Blueprint bp in m_blueprintManagerInstance.m_blueprintList)
        {
            foreach (BP_Object obj in bp.m_objectList)
                objs.Add(obj);
        }
        foreach (BP_Object bp in objs)
        {
            uint parentBPIndex = bp.m_parentBP.indexOfBlueprint;
            //  type에 맞게 적절하게 InfoList의 List에 저장
            switch (bp.m_type)
            {
                case BP_Object.type.Gear:
                    BP_Gear bp_gear = bp.GetComponent<BP_Gear>();
                    BP_GearInfo gearInfo = new BP_GearInfo(bp.transform, bp.m_instanceID, BP_Object.type.Gear, parentBPIndex);
                    
                    int gcount = 0;
                    gearInfo.childJointIDList = new int[bp_gear.m_childJointList.Count];
                    foreach (BP_Joint childJoint in bp_gear.m_childJointList)
                    {
                        gearInfo.childJointIDList[gcount++] = childJoint.m_instanceID;
                    }
                    if (bp_gear.m_parentObj)
                        gearInfo.parentObjID = bp_gear.m_parentObj.m_instanceID;
                    infoList.gearInfoList.Add(gearInfo);
                    break;
                case BP_Object.type.Link:
                    BP_Link bp_link = bp.GetComponent<BP_Link>();
                    BP_LinkInfo linkInfo = new BP_LinkInfo(bp.transform, bp.m_instanceID, BP_Object.type.Link, parentBPIndex);

                    int lcount = 0;
                    linkInfo.childJointIDList = new int[bp_link.m_childJointList.Count];
                    foreach (BP_Joint childJoint in bp_link.m_childJointList)
                    {
                        linkInfo.childJointIDList[lcount++] = childJoint.m_instanceID;
                    }

                    linkInfo.startJointID = bp_link.m_startJoint.m_instanceID;
                    linkInfo.endJointID = bp_link.m_endJoint.m_instanceID;

                    infoList.linkInfoList.Add(linkInfo);
                    break;
                case BP_Object.type.Joint:
                    BP_Joint bp_joint = bp.GetComponent<BP_Joint>();
                    BP_JointInfo jointInfo = new BP_JointInfo(bp.transform, bp.m_instanceID, BP_Object.type.Joint, parentBPIndex);

                    jointInfo.parentObjID = bp_joint.m_parentLink.m_instanceID;
                    if (bp_joint.m_attachedObj)
                        jointInfo.attachedObjID = bp_joint.m_attachedObj.m_instanceID;
                    jointInfo.jointType = bp_joint.m_jointType;

                    infoList.jointInfoList.Add(jointInfo);
                    break;
                case BP_Object.type.Shaft:
                    BP_Shaft bp_shaft = bp.GetComponent<BP_Shaft>();
                    BP_ShaftInfo shaftInfo = new BP_ShaftInfo(bp.transform, bp.m_instanceID, BP_Object.type.Shaft, parentBPIndex);

                    shaftInfo.shaftType = bp_shaft.m_shaftType;
                    shaftInfo.childObjectIDList = new int[bp_shaft.m_childObjList.Count];
                    int scount = 0;
                    foreach (BP_Object childObject in bp_shaft.m_childObjList)
                    {
                        shaftInfo.childObjectIDList[scount++] = childObject.m_instanceID;
                    }
                    infoList.shaftInfoList.Add(shaftInfo);
                    break;
                case BP_Object.type.PinFollower:
                    break;
                case BP_Object.type.SlottedBar:
                    BP_SlottedBar bp_slottedBar = bp.GetComponent<BP_SlottedBar>();
                    BP_SlottedBarInfo slottedBarInfo = new BP_SlottedBarInfo(bp.transform, bp.m_instanceID, BP_Object.type.SlottedBar, parentBPIndex);

                    int slcount = 0;
                    slottedBarInfo.childJointIDList = new int[bp_slottedBar.m_childJointList.Count];
                    foreach (BP_Joint childJoint in bp_slottedBar.m_childJointList)
                    {
                        slottedBarInfo.childJointIDList[slcount++] = childJoint.m_instanceID;
                    }

                    slottedBarInfo.startJointID = bp_slottedBar.m_startJoint.m_instanceID;
                    slottedBarInfo.endJointID = bp_slottedBar.m_endJoint.m_instanceID;

                    infoList.slottedBarInfoList.Add(slottedBarInfo);
                    break;
                case BP_Object.type.EndEffector:
                    break;
            }
        }

        //string saveData = JsonHelper.ToJson(infoList.gearInfoList.ToArray(), true);
        //  Json 형식 text로 변환
        string saveData = JsonMapper.ToJson(infoList);
        //  Resources의 Saves 폴더에 저장
        File.WriteAllText(Application.dataPath + "/Resources/Saves/" + fileName + ".json", saveData);
        print("Save data");
    }

    public void loadAll()
    {
        #region 옛날 방식
        ////  load objects
        //string[] files = Directory.GetFiles(Application.dataPath + "/Saves/");
        //BP_Object[] objs = new BP_Object[files.Length];

        ////  instantiate
        //foreach (string file in files)
        //{
        //    if (Path.GetExtension(file) == ".json")
        //    {
        //        string jsonData = File.ReadAllText(file);
        //        print(jsonData);
        //        JsonData getData = JsonMapper.ToObject(jsonData);
        //        int gearType = int.Parse(getData["m_gearType"].ToString());
        //        bool isSwitch = bool.Parse(getData["m_switch"].ToString());
        //        print(getData["m_parentBP"]["instanceID"].ToString());
        //        int instanceid = int.Parse(getData["m_parentBP"]["instanceID"].ToString());
        //        print("gear Type: " + getData["m_gearType"].ToString());
        //        print(", switch: " + getData["m_switch"].ToString());
        //        print(", instance id: " + getData["m_parentBP"]["instanceID"].ToString());

        //        //BP_Gear obj = JsonUtility.FromJson<BP_Gear>(file);
        //        //switch(obj.m_type)
        //        //{
        //        //    case BP_Object.type.Gear:
        //        //        break;
        //        //    case BP_Object.type.Link:
        //        //        break;
        //        //    case BP_Object.type.Joint:
        //        //        break;
        //        //    case BP_Object.type.Shaft:
        //        //        break;
        //        //    case BP_Object.type.PinFollower:
        //        //        break;
        //        //    case BP_Object.type.SlottedBar:
        //        //        break;
        //        //    case BP_Object.type.EndEffector:
        //        //        break;
        //        //}
        //    }
        //}
        #endregion

        //string fileName = Application.dataPath + "Saves/38.30442";
        //  Json 파일을 string으로 가져옴
        TextAsset txt = Instantiate(Resources.Load("Saves/" + fileName)) as TextAsset;
        string jsonTxt = txt.text;
        //print(jsonTxt);
        //JsonReader jsonReader = new JsonReader(jsonTxt);

        #region 1차 작업: BP_Object들을 생성 후 transform, Blueprint에 입력.
        BP_InfoList infoList = JsonUtility.FromJson<BP_InfoList>(jsonTxt);
        //  BP_Gear 생성
        BP_GearInfo[] gInfoList = infoList.gearInfoList.ToArray();
        foreach (BP_GearInfo gInfo in gInfoList)
        {
            BP_Gear bp_gear = Instantiate<BP_Gear>(m_gearPrefab);
            bp_gear.setPosition(gInfo.transform.getFloatPos());
            bp_gear.transform.rotation = gInfo.transform.getFloatRot();
            bp_gear.transform.localScale = gInfo.transform.getFloatScale();
            bp_gear.addThisToBP(FindObjectOfType<BlueprintManager>().getBlueprintByIndex(gInfo.parentBPIndex));
            bp_gear.m_instanceID = gInfo.instanceID;
        }
        //  BP_Shaft 생성
        BP_ShaftInfo[] sInfoList = infoList.shaftInfoList.ToArray();
        foreach (BP_ShaftInfo sInfo in sInfoList)
        {
            BP_Shaft bp_shaft = Instantiate<BP_Shaft>(m_shaftPrefab);
            bp_shaft.transform.position = sInfo.transform.getFloatPos();
            bp_shaft.transform.rotation = sInfo.transform.getFloatRot();
            bp_shaft.transform.localScale = sInfo.transform.getFloatScale();
            bp_shaft.addThisToBP(FindObjectOfType<BlueprintManager>().getBlueprintByIndex(sInfo.parentBPIndex));
            bp_shaft.m_instanceID = sInfo.instanceID;
        }
        //  BP_Link 생성
        BP_LinkInfo[] lInfoList = infoList.linkInfoList.ToArray();
        foreach (BP_LinkInfo lInfo in lInfoList)
        {
            BP_Link bp_link = Instantiate<BP_Link>(m_linkPrefab);
            bp_link.transform.position = lInfo.transform.getFloatPos();
            bp_link.transform.rotation = lInfo.transform.getFloatRot();
            bp_link.transform.localScale = lInfo.transform.getFloatScale();
            bp_link.addThisToBP(FindObjectOfType<BlueprintManager>().getBlueprintByIndex(lInfo.parentBPIndex));
            bp_link.m_instanceID = lInfo.instanceID;
        }
        //  BP_SlottedBar 생성
        BP_SlottedBarInfo[] slInfoList = infoList.slottedBarInfoList.ToArray();
        foreach (BP_SlottedBarInfo slInfo in slInfoList)
        {
            BP_SlottedBar bp_slottedBar = Instantiate<BP_SlottedBar>(m_SlottedBarPrefab);
            bp_slottedBar.transform.position = slInfo.transform.getFloatPos();
            bp_slottedBar.transform.rotation = slInfo.transform.getFloatRot();
            bp_slottedBar.transform.localScale = slInfo.transform.getFloatScale();
            bp_slottedBar.addThisToBP(FindObjectOfType<BlueprintManager>().getBlueprintByIndex(slInfo.parentBPIndex));
            bp_slottedBar.m_instanceID = slInfo.instanceID;
        }
        //  BP_Joint 생성
        BP_JointInfo[] jInfoList = infoList.jointInfoList.ToArray();
        foreach (BP_JointInfo jInfo in jInfoList)
        {
            BP_Joint bp_joint = Instantiate<BP_Joint>(m_jointPrefab);
            bp_joint.transform.position = jInfo.transform.getFloatPos();
            bp_joint.transform.rotation = jInfo.transform.getFloatRot();
            bp_joint.transform.localScale = jInfo.transform.getFloatScale();
            bp_joint.addThisToBP(FindObjectOfType<BlueprintManager>().getBlueprintByIndex(jInfo.parentBPIndex));
            bp_joint.m_instanceID = jInfo.instanceID;

            //  BP_Joint의 setJointType으로 joint type 설정.
            bp_joint.setJointType(jInfo.jointType);
        }

        #endregion

        #region  2차 작업: 생성된 BP_Object들을 BP_Info의 정보에 맞게 연결해줌
        foreach (BP_ShaftInfo sInfo in sInfoList)
        {
            BP_Shaft curShaft = null;
            foreach (BP_Shaft shaft in FindObjectsOfType<BP_Shaft>())
            {
                if (shaft.m_instanceID == sInfo.instanceID)
                {
                    curShaft = shaft;
                    break;
                }
            }
            if (curShaft == null)
                print("SaveLoadManager: shaft finding error");
            else
            {
                foreach (BP_Object obj in FindObjectsOfType<BP_Object>())
                {
                    //  Shaft의 Child object를 찾는다
                    foreach(int childObjID in sInfo.childObjectIDList)
                    {
                        //  찾으면 Shaft에 연결한다
                        if(childObjID == obj.m_instanceID)
                        {
                            if (obj.GetComponent<BP_Joint>())
                            {
                                print("asrfasdf");
                                BP_Joint attachedJoint = obj.GetComponent<BP_Joint>();
                                attachedJoint.m_attachedObj = curShaft;
                                curShaft.m_childObjList.Add(attachedJoint);
                                attachedJoint.m_attachedObj = curShaft;
                            }
                        }
                    }
                }
            }
        }
        foreach (BP_GearInfo gInfo in gInfoList)
        {
            BP_Gear curGear = null;
            foreach (BP_Gear gear in FindObjectsOfType<BP_Gear>())
            {
                if (gear.m_instanceID == gInfo.instanceID)
                {
                    curGear = gear;
                    break;
                }
            }
            if (curGear == null)
                print("SaveLoadManager: gear finding error");
            else
            {
                foreach (BP_Object obj in FindObjectsOfType<BP_Object>())
                {
                    //  Object가 Gear의 부모 Object라면
                    if (gInfo.parentObjID == obj.m_instanceID)
                    {
                        if (curGear.m_instanceID == gInfo.instanceID)
                        {
                            curGear.parenting(obj.gameObject);
                        }
                    }
                    else
                    {
                        foreach (int childObjID in gInfo.childJointIDList)
                        {
                            //  Object가 Gear의 자식 오브젝트라면
                            if (obj.m_instanceID == childObjID)
                            {
                                BP_Joint childJoint = obj.GetComponent<BP_Joint>();
                                curGear.m_childJointList.Add(childJoint);
                                childJoint.m_attachedObj = curGear;
                            }
                        }
                    }
                }
            }
        }
        //  Link 정보들을 하나씩 순회
        foreach (BP_LinkInfo lInfo in lInfoList)
        {
            //  만들어진 Link들에서 이 LinkInfo에 맞는 Link를 찾음
            BP_Link curLink = null;
            foreach (BP_Link link in FindObjectsOfType<BP_Link>())
                if (link.m_instanceID == lInfo.instanceID)
                    curLink = link;

            //  Link를 못찾으면 에러
            if (curLink == null)
                print("SaveLoadManager: link finding error");
            else
            {
                //  Joint를 돌면서
                foreach (BP_Joint joint in FindObjectsOfType<BP_Joint>())
                {
                    //  Joint가 Link의 Start or End joint라면
                    //  할당 후 Joint의 부모 오브젝트에도 이 Link를 할당.
                    if (lInfo.startJointID == joint.m_instanceID)
                    {
                        curLink.m_startJoint = joint;
                        joint.m_parentLink = curLink;
                    }
                    else if (lInfo.endJointID == joint.m_instanceID)
                    {
                        curLink.m_endJoint = joint;
                        joint.m_parentLink = curLink;
                    }
                    //  현재 Link에 붙어있는 Joint라면
                    else
                    {
                        foreach (int childJointID in lInfo.childJointIDList)
                        {
                            if (childJointID == joint.m_instanceID)
                            {
                                curLink.m_childJointList.Add(joint);
                                joint.m_attachedObj = curLink;
                                break;
                            }
                        }
                    }
                }
            }
        }
        //  SlottedBar 정보들을 하나씩 순회
        foreach (BP_SlottedBarInfo slInfo in slInfoList)
        {
            //  만들어진 Link들에서 이 LinkInfo에 맞는 Link를 찾음
            BP_SlottedBar curSlottedBar = null;
            foreach (BP_SlottedBar slottedBar in FindObjectsOfType<BP_SlottedBar>())
                if (slottedBar.m_instanceID == slInfo.instanceID)
                    curSlottedBar = slottedBar;

            //  Link를 못찾으면 에러
            if (curSlottedBar == null)
                print("SaveLoadManager: link finding error");
            else
            {
                //  Joint를 돌면서
                foreach (BP_Joint joint in FindObjectsOfType<BP_Joint>())
                {
                    //  Joint가 Link의 Start or End joint라면
                    //  할당 후 Joint의 부모 오브젝트에도 이 Link를 할당.
                    if (slInfo.startJointID == joint.m_instanceID)
                    {
                        curSlottedBar.m_startJoint = joint;
                        joint.m_parentLink = curSlottedBar;
                    }
                    else if (slInfo.endJointID == joint.m_instanceID)
                    {
                        curSlottedBar.m_endJoint = joint;
                        joint.m_parentLink = curSlottedBar;
                    }
                    //  현재 Link에 붙어있는 Joint라면
                    else
                    {
                        foreach (int childJointID in slInfo.childJointIDList)
                        {
                            if (childJointID == joint.m_instanceID)
                            {
                                curSlottedBar.m_childJointList.Add(joint);
                                joint.m_attachedObj = curSlottedBar;
                                break;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        m_blueprintManagerInstance.refresh();

        //while(jsonReader.Read())
        //{
        //    string type = jsonReader.Value != null ?
        //        jsonReader.Value.GetType().ToString() : "";
        //    Debug.Log(jsonReader.Token + "|" + jsonReader.Value + "|" + type);
        //}
        //List<BP_GearInfo> gearInfos = new List<BP_GearInfo>(JsonMapper.ToObject<BP_GearInfo[]>(jsonReader));

        //int count = 0;
        //foreach (BP_GearInfo gearInfo in gearInfos)
        //{
        //    print("GearInfo " + count);
        //    print("position: " + gearInfo.transform.dPosition);
        //    print("rotation: " + gearInfo.transform.dRotation);
        //    print("scale: " + gearInfo.transform.dScale);
        //    print("ID: " + gearInfo.instanceID);
        //    print("parent Blueprint index: " + gearInfo.parentBPIndex);
        //    if(gearInfo.parentObjID != 0)
        //        print("parent Object ID: " + gearInfo.parentObjID);
        //}

        print("load data success");
    }

    // Use this for initialization
    void Start()
    {
        //string json = JsonUtility.ToJson(this);
        ////File.Create(Application.dataPath + "/Saves/data.json");
        //File.WriteAllText(Application.dataPath + "/Saves/data.json", json);
        //print(Application.dataPath + "/Saves/ 에 데이터 저장?");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        if (SaveOrLoad)
            saveAll();
        else
            loadAll();
    }

    #region 안 씀
    public void getDownInput(Vector3 hitPoint)
    {
    }

    public void getUpInput(Vector3 hitPoint)
    {
    }

    public void getMotion(Vector3 hitPoint)
    {
    }
    #endregion
}
