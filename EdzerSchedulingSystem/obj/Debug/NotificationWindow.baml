   M S B A M L   `   `   `  ����P  LEdzerSchedulingSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:EdzerSchedulingSystemEdzerSchedulingSystem  5   	   .    (EdzerSchedulingSystem.NotificationWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:EdzerSchedulingSystem     5   	   -   5          �Name$  notificationWindow��5	   	      NoBorderWindow#��[  5        PrimaryFont#��[ 6B   $	��200��5   	   $	��350��6    J� Opacity$ 0Q�5	   %   ��5   "   Z� .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $��* �6   .+�5   
   �� $	 0.8Q�6   
���   �$��2��6/   �� 5   
   -   5         ��Name$ btnClose��5        imgClose#��[ 69     WhiteButton#��[ 6]   $��20��5      $��20��6   $��0q�6(   $��0��6<   $��Right=�6I   $��	0 10 10 0q�6e   ��5   #   Z� 5   x   �� 5   
   -   5         ��Name$ txtTitle��5      $
��Bold"�6&   $��20(�68   $��0��6F   $
��Left=�6S   $
��Wrap=�6n   $	��280��6�     PrimaryTextColor#��[ 5      $��Center=�6C   $��	10 10 0 0q�6^   �� 5   
   -   5        $ 
txtMessage��5      $
��Wrap=�6(   $��14(�6<   $��1��6J   #��[ 5      $
��10 5q�6C   5       5"      