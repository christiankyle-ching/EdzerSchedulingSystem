   M S B A M L   `   `   `  ����P  LEdzerSchedulingSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:EdzerSchedulingSystemEdzerSchedulingSystem  5   	   +    %EdzerSchedulingSystem.ChangelogWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:EdzerSchedulingSystem     5   	   $	��700��5      $	��500��6'   $	��600��63   $	��500��6C     � Title$  	Changelog��5   	    � WindowStartupLocation$ CenterScreen=�5	   	      PrimaryFont#��[  6.   .��5
      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $��* �6   .+�5   
   �� .+�5      �� $��Price History��6     	TitleText#��[ 6-     SecondaryTextColor#��[ 6P   $
��20 5q�6�   �� 5      $
��10 0q�6   � 5      $��20 10q�6    � ColumnDefinitions 5      �� 5      $
��auto �6'   �� 5      $��* �6'    5      �� 5      $
��auto �6$   �� 5      $
��auto �6$   �� 5       $
��auto �6$   .+�5$      �� $��Type:��6   $��Center=�6)   $	��0 8q�6D   $��0��5%      $��0��6)   �� 5&      -   5         ��Name$ cbType��5&      $��Center=�6+   $��10 0 0 0q�6F   $��0��5'      $��1��6&   $	��150��66   $
��Left=�6B   5(      �� 5,      $��From:��6   $��Center=�6)   $	��0 8q�6D   $��1��5-      $��0��6)   �� 5.      $��1��6   $��1��6*    �� Orientation$ 
Horizontal=�6:   $
��Left=�6S   .+�5/      (  "System.Windows.Controls.DatePicker  -   5          Name$ 
dpFromDate��5/   !   $��Center=�65   $	��150��6P   $��10 0 0 0q�6\   �� 50      -   5         ��Name$ chkStart��50      $��	Beginning��61   $��Center=�6E   $
��10 0q�6`   51   A   52      �� 56      $	��To:��6   $��Center=�6'   $	��0 8q�6B   $��2��57      $��0��6)   �� 58      $��2��6   $��1��6*   $ 
Horizontal=�6:   $
��Left=�6S   .+�59        -   5        $ dpToDate��59   !   $��Center=�63   $	��150��6N   $��10 0 0 0q�6Z   �� 5:      -   5        $ chkEnd��5:      $	��End��6/   $��Center=�6=   $
��10 0q�6X   5;   ?   5<      �� 5?      -   5         ��Name$ btnGenerate��5?      $
��Load��6.     PrimaryButtonStyle#��[ 6=   $��3��5@      $��2��6*   $��Bottom=�6>   $��Right=�6Y   {�.  5A   +   5C      5D      &   System.Windows.Controls.DataGrid  5F   
   -   5        	  Name$	 dgReport��5F      $��10q�6%   
   
IsReadOnly
 .    	RowHeight$ 20Q�6C   $��1��5G      {�.  ��62   ��    Columns 5H      0  *System.Windows.Controls.DataGridTextColumn  5I      ,  &System.Windows.Controls.DataGridColumn   Header$
 Name��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6L   1  +System.Windows.Controls.DataGridBoundColumn   Binding 63   �� *Type+6L     5J      $ Price��6%   $ * 6U    64   �� *PricePerHour+6U     5K      $ DateEffective��6%   $ * 6U    6<   �� *Date+6U   5M      5O      5P      