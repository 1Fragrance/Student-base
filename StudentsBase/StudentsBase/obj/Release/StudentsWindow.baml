   M S B A M L   `   `   `  ����   StudentsBase!    StudentsBase.StudentsWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$  Students��$	��800��$	��705��$	��800��$	��600�����#008B8B.����  �� Padding$ 10q�._�� .+�o� $��19��$��
-9,-9,-9,0q�$	��Top=�.@�m� $
��File��.@�m� -   $��	newButton��$	��New�� m� Icon �� -   $��newButtonImage��m� -   $��
openButton��$
��Open�� �� -   $��openButtonImage��m� -   $��
saveButton��$
��Save�� �� -   $��saveButtonImage���� m� -   $��
exitButton��$
��Exit�� �� -	   $��exitButtonImage��m� $��Plugins��.@�m� -
   $��	runButton��$	��Run�� �� -   $��runButtonImage���� m� -   $��openPlMenuButton��$��Plugins Menu�� �� -   $��menuButtonImage���� -    ��Name$ EditingPanel��$	��148��$	��549��$
��0,30q�$
��Left=�$	��Top=�.+��� $
��45,0q�$��Center=�$	�� 16(����	#FFF0EBE8$��	Underline{��� $�Adding���� $	��6,5q�$	�� 12(����	#FFF0EBE8$��Name:���� -    ��Name$ NameBox��$	��136��$��Center=��� $	��6,0q�$	�� 12(����	#FFF0EBE8$��Surname:���� -   $ 
SurnameBox��$	��136��$��Center=��� $	��6,0q�$	�� 12(����	#FFF0EBE8$��Middlename:���� -   $ 
Middlename��$	��136��$��Center=��� $	��6,0q�$	�� 12(����	#FFF0EBE8$��Year of receipt:���� -   $ YearBox��$	��136��$��Center=��� -    ��Name$ 	HeadCheck��$
��6,10q�$��Head of the group?�����	#FFF0EBE8�� $	��6,0q�$	�� 12(����	#FFF0EBE8$��Group:���� -    ��Name$ 
GroupCombo��$	��6,0q��� $	��6,0q�$	�� 12(����	#FFF0EBE8$��
Old Group:���� -   $ OldGroupCombo��$	��6,0q��� $	��6,0q�$	�� 12(����	#FFF0EBE8$��Load Photo:���� -   $ FileNameTextBox��$	��136��$��20��$	��6,0q�$
��Left=�$	��Top=�{�.  �� -    ��Name$ button1��$��50��$��20��$	��6,4q�$
��Left=�$	��Top=�$��Browse���� -    ��Name$ Table��$��92��$	��6,5q�	 �� View	 �� .P��� 
 �� Width$
 65��$��Subject�� �� DisplayMemberBinding �� *	Subject+�� $
 66��$
��Mark�� �� *Mark+�� -   $	��130��$��21��$	��6,5q�$
��Left=�$	��Top=�$��Add Subject���� -   $ Delete_Mark��$	��130��$��21��$	��6,5q�$
��Left=�$	��Top=�$��Delete Selected Subject��{�.  �� -   $ Clear_Field��$	��130��$��21��$	��6,5q�$
��Left=�$	��Top=�$��Clear Fields���� -   $
 List��$��190,30,10,183q�	 �� .P��� $	
 140��$��Surname�� �� *	Surname+�� $	
 140��$
��Name�� �� *Name+�� $	
 140��$��
Middlename�� �� *
Middlename+�� $	
 148��$��Group�� �� *group_number+����  �� 
TargetType" ��.���  �� Property � �� �� Value$
 Left=��� -   $ 	AddButton��$	��138��$��57��$��0,0,430,104q�$��Right=�$��Bottom=�$��Add Student���� -   $ 
SortButton��$	��138��$��57��$��
0,0,430,35q�$��Right=�$��Bottom=�$��Sort Students by Name���� -   $ GroupButton��$	��154��$��57��$��	0,0,10,30q�$��Right=�$��Bottom=�$��Sort Students by Group���� -    $ DeleteButton��$	��154��$��57��$��0,0,220,104q�$��Right=�$��Bottom=�$��Delete Student��{�.  �� -!   $ ChangeButton��$	��154��$��57��$��
0,0,10,104q�$��Right=�$��Bottom=�$��Change Information��{�.  �� $	��154��$��22��$��
0,0,220,65q�$��Right=�$��Bottom=�$	�� 11(����	#FFF0EBE8$ ��Search Student by Surname:���� -"   $ SearchField��$	��154��$��30��$��
0,0,220,35q�$��Right=�$��Bottom=�