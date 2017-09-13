using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TestLinq : MonoBehaviour
{
    public List<Student> listStudent = new List<Student>();
    public List<Courses> listCourses = new List<Courses>();

    public List<Student> showListStudent = new List<Student>();
    public List<Courses> showListCourses = new List<Courses>();

    public List<int> listInt = new List<int>();

    /*
  Student类：表示学生，包括学号、姓名及班级
    Courses类：表示学生选择的课程，包括学号、课程名称及学时数
    DataCreator类：静态类，通过GenerateData方法产生示例数据
学号    姓名    班级    课程名称    学时 
003        王五    二班    经济学    20 
003        王五    二班    企业管理    20 
003        王五    二班    财务管理    30 
002        李四    一班    历史    20 
002        李四    一班    政治    20 
002        李四    一班    语文    30 
001        张三    一班    数学    20 
001        张三    一班    语文    20 
001        张三    一班    物理    15
*/
    // Use this for initialization
    void Start()
    {
        listStudent.Add(new Student(1001, "张三", "一班"));
        listStudent.Add(new Student(1003, "王五", "二班"));
        listStudent.Add(new Student(1002, "李四", "一班"));

        listCourses.Add(new Courses(1001, "物理", 15));
        listCourses.Add(new Courses(1001, "语文", 20));
        listCourses.Add(new Courses(1001, "数学", 20));
        listCourses.Add(new Courses(1002, "语文", 30));
        listCourses.Add(new Courses(1002, "政治", 20));
        listCourses.Add(new Courses(1002, "历史", 20));
        listCourses.Add(new Courses(1003, "财务管理", 30));
        listCourses.Add(new Courses(1003, "企业管理", 20));
        listCourses.Add(new Courses(1003, "经济学", 20));
        listInt = new List<int>(new int[] { 5, 8, 1, 3, 4, 2, 9 });

        int i_1X = 0;
        listInt = (from i_X in listInt
                   orderby i_X ascending
                   select i_X
                   ).ToList();
        listInt = listInt.Take(3).ToList();

        Dictionary<int, Student> testDic = new Dictionary<int, Student>();

        testDic = (from stdt in listStudent
                   orderby stdt.s_ID descending
                   select stdt).ToDictionary(stdt => stdt.s_ID, stdt => stdt);
        Student[] aryStudent = (from stdt in listStudent
                                orderby stdt.s_ID descending
                                select stdt).ToArray();

        //.ToDictionary(stdt => stdt.s_ID, stdt => stdt);
        foreach (KeyValuePair<int, Student> item in testDic)
        {
            Debug.Log("这是一个测试" + item.Key + "\t\t名字：" + item.Value.s_Name);
        }
        if (listInt.Contains(3))
        {
            Debug.Log("存在3");
        }

        if (listInt.Contains(6) == false)
        {
            Debug.Log("不存在6");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Rank_ascendingWhere()
    {
        showListStudent = (from stdt in listStudent
                           where stdt.s_Class == "一班"
                           orderby stdt.s_ID ascending 
                           select stdt).ToList();

    }

    public void Rank_ascending()
    {
        showListStudent = (from stdt in listStudent
                           orderby stdt.s_ID ascending
                           select stdt).ToList();

    }

    public void Rank_descending()
    {
        showListStudent = (from stdt in listStudent
                           orderby stdt.s_ID descending
                           select stdt).ToList();
    }
}
[Serializable]
public class Student
{
    public int s_ID;
    public string s_Name;
    public string s_Class;
    public Student()
    { }
    public Student(int id, string name, string class_N)
    {
        s_ID = id;
        s_Name = name;
        s_Class = class_N;
    }
}
[Serializable]
public class Courses
{
    public int s_ID;
    public string s_CoursesName;
    public int s_CoursesTime;
    public Courses()
    { }
    public Courses(int id, string cName, int cTime)
    {
        s_ID = id;
        s_CoursesName = cName;
        s_CoursesTime = cTime;
    }
}

