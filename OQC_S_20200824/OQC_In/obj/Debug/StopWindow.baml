   M S B A M L   `   `   `  ����B  >OQC_IN, Version=1.8.12.1, Culture=neutral, PublicKeyToken=nullclr-namespace:OQC_INOQC_IN  5   	       OQC_IN.StopWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:OQC_IN     5   	     � Title$  空闲中...��5   	   $��367.891��6   $��493.957��6)    � Icon$ logo.ico��69    � 
ResizeMode$ NoResize=�6I    � ShowInTaskbar .   � WindowStartupLocation$ CenterScreen=�6u   .��5	      �� $��Center=�6   .+�5
   
   �� $��上次读码时间��6   $��Center=�6"   $	��0,5q�6?   �� 5   
   $��26(�6<   $
��Bold"�6J   $��Center=�6\   ��6   �� *StopHelper.LastDateStr+6{   �� 5   
   $��停机时长��6   $��Center=�6    $	��0,5q�6=   �� 5   
   $��60(�6<   $
��Bold"�6J   ���	#FFD90303$��Center=�6s   ��6   �� *StopHelper.StopTimeStr+6�   �� 5   
   $��停机原因��6   $��Center=�6    $	��0,5q�6=   �� 5   
   $	��200��6    �� Text$ 请选择停机原因��6   ��6.   �� *
StopMsgs+ �� SelectedIndex 6O   �� *StopMsgIndex+6x   �� 5   
   $��恢复��6N   $	��200��6[   $��26��6g   $��10q�6s   {�6   �� *	BtnShow+��6/   �� *
SetCommand+6s   5      5      