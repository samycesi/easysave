   M S B A M L   `   `   `  ����H  DRemoteMonitor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:RemoteMonitorRemoteMonitor  5   	   A%clr-namespace:RemoteMonitor.ViewModelRemoteMonitor.ViewModel  5   	       RemoteMonitor.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:RemoteMonitor     5   	   ;	viewModel%clr-namespace:RemoteMonitor.ViewModel     5   	     � Title$  
MainWindow��5
   	   $	��600��6   $
��1000��6)   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��250 �6   �� 5       � RowDefinitions 5   
   �� 5      .+�5   
   �� $��1��6   ���	#FFFFECEC.+�5      �� $��0��6   .+�5      �� $��EasySave Monitor��6   $��Center=�5      $
��Left=�5      
���   �$��25(�5      $��	20 20 0 0q�5      �� 5       $��0 20 0 0q�6   .+�5!      	� $��Config��6"   $��50��5"      
���   �$��20(�5$         	MenuStyle#��[  5%       d� 	IsChecked$
 TrueY�5&      ��5'      �� *ConfigViewCommand+	� 5(      $��Monitor��6"   $��50��5)      
���   �$��20(�5+      #��[  5,      ��5-      �� *MonitorViewCommand+5.      5/      50      �� 53   
   $��1��6   $��1��54      $��0q�55      ��56      �� *CurrentView+57      58      