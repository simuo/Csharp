   M S B A M L   `   `   `  ���� B  >OQC_IN, Version=1.8.12.1, Culture=neutral, PublicKeyToken=nullclr-namespace:OQC_INOQC_IN      OQC_IN.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:OQC_IN       � Title$  	主界面��$	��692��$
��1000�� � WindowStartupLocation$ CenterScreen=� � WindowState$ 	Maximized=� � Icon$ logo.ico��.���  � ColumnDefinitions �� $��* ��� $	��300 �.+�� $��0��$��20q� � RowDefinitions �� $��* ��� $	��160 � �� $	��.5* ��� $	��.5* �.+��� $��Center=�$��10q�$��18(�$
��Bold"����� *Lines[0].Name+�� $��20,40,20,20q� �� CornerRadius$ 20������ *Lines[0].StateColor+._��� $��Center=�.+��� $��50(�$��Center=�$��Center=����� *Lines[0].StateText+���� *Lines[0].StateTextColor+��  �� TextAlignment$ Center=�$��16(�$��5,40,5,5q�$��Center=����� *Lines[0].NgText+���� *Lines[0].StateTextColor+�� $��1��$��Center=�$��10q�$��18(�$
��Bold"����� *Lines[1].Name+�� $��1��$��20,40,20,20q�$ 20������ *Lines[1].StateColor+._��� $��Center=�.+��� $��50(�$��Center=�$��Center=����� *Lines[1].StateText+���� *Lines[1].StateTextColor+�� $ Center=�$��16(�$��5,40,5,5q�$��Center=����� *Lines[1].NgText+���� *Lines[1].StateTextColor+�� $��重置计数��$��70��$��22��$��5q�$��1��$��Bottom=�$��Right=����� *ClearLogCommand1+�� $��1��$��10,20,10,10q�.+��� $��Center=�$��32(��� ��� *Lines[0].OK+ �� $�/�� �� ��� *Lines[0].Total+�� $��OK数/总数��$��Center=�$��16(���  �� Orientation$ 
Horizontal=�$��0,15,0,0q�$��Center=�.+��� $ Right=�$��NG数：��$��16(��� $��16(��� ��� *Lines[0].NG+ �� $�/�� �� ��� *Lines[0].Total+�� $��重置计数��$��70��$��22��$��5q�$��1��$��1��$��Bottom=�$��Right=����� *ClearLogCommand2+�� $��1��$��1��$��10,20,10,10q�.+��� $��Center=�$��32(��� ��� *Lines[1].OK+ �� $�/�� �� ��� *Lines[1].Total+�� $��OK数/总数��$��Center=�$��16(��� $ 
Horizontal=�$��0,15,0,0q�$��Center=�.+��� $ Right=�$��NG数：��$��16(��� $��16(��� ��� *Lines[1].NG+ �� $�/�� �� ��� *Lines[1].Total+� $��1�� �� $��60 ��� $��* ��� $	��300 �.+�� $��0,0,10,0q� �� $��* ��� $��* ��� $��* ��� $��* �.+��� $��Center=�$��	0,10,0,10q�.+�A� $��Center=�$��14��$��5,0,5,0q�
���   �$��Center=�$��14������ *IOCard.TriggerStateColor1+�� $��
1线信号��$��0,10,0,0q��� $��1��$��Center=�$��	0,10,0,10q�.+�A� $��Center=�$��14��$��5,0,5,0q�
���   �$��Center=�$��14������ *IOCard.TriggerStateColor2+�� $��
2线信号��$��0,10,0,0q��� $��2��$��Center=�$��	5,10,0,10q�.+�A� $��Center=�$��14��$��5,0,5,0q�
���   �$��Center=�$��14������ *LineOut.LinkState+�� $��线尾连接��$��0,10,0,0q��� $��3��$��Center=�$��	5,10,0,10q�.+�A� $��Center=�$��14��$��5,0,5,0q�
���   �$��Center=�$��14������ *SocketHelper.StateColor+�� $��读码连接��$��0,10,0,0q��� $��1��$��10,50,10,10q�.+��� $��Center=�$��32(��� ��� *Count.Count.OK+ �� $�/�� �� ��� *Count.Count.Total+�� $��OK数/总数��$��Center=�$��16(��� $ 
Horizontal=�$��0,15,0,0q�.+��� $	��150��$ Right=�$��NG数：��$��16(��� $��16(��� ��� *Count.Count.NG+ �� $�/�� �� ��� *Count.Count.Total+�� $ 
Horizontal=�$��0,35,0,0q�.+��� $	��150��$ Right=�$��Trace上抛数：��$��14(��� $��14(��� ��� *Count.Count.TraceOk+ �� $�/�� �� ��� *Count.Count.TraceTotal+�� $ 
Horizontal=�$��0,5,0,0q�.+��� $	��150��$ Right=�$��Trace NG数：��$��14(��� $��14(��� ��� *Count.Count.TraceNG+ �� $�/�� �� ��� *Count.Count.TraceTotal+�� $��1��$��Bottom=�$��10,10,10,25q�y��� *Stop.Enable+.+��� -   	 � Cursor$
	 Hand��
���  ��$��16(�$
��Bold"����� *Stop.StopMsgStr+�� $��1��$ 
Horizontal=�$��Bottom=�$
��10,5q�y��� *Stop.Enable+.+��� $��空闲时长：���� $
��Bold"����� *Stop.StopTimeStr+�� $�� (���� ���� *Stop.StopDelayStr+�� $��)���� $��1��$ 
Horizontal=�$��Right=�$��Bottom=�$��0,0,10,0q�.+��� $��重置计数��$
��10,3q����� *ClearLogCommand+�� $��2��$��Stretch=�$��
0,10,10,10q�$��Stretch=����� *
LogsData+� $��2��$��Bottom=�$��10,15,10,10q�$��Stretch=�y��� *MoniVisibility+.+��� -   $��
IO1 模拟��$��80��$
��10,3q�$��0,0,90,3q�$��Right=��� -   $��
IO2 模拟��$��80��$
��10,3q�$��0,0,0,3q�$��Right=�