   M S B A M L   `   `   `  ���� D  @Sentiment, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Sentiment	Sentiment  9!clr-namespace:Sentiment.ViewModelSentiment.ViewModel      Sentiment.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:Sentiment     /c!clr-namespace:Sentiment.ViewModel       � Title$  
MainWindow��$	��600��$	��550�� � 	Resources "   Sentiment.ViewModel.NameList   NameListData��%   &	          .��� $��10q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��80 �.+���  �� Orientation$ Vertical=�$��0��.+��  � ColumnDefinitions �� $	��100 ��� $	��200��.+��� $��0��  _licensePath#��[ �� $��1��$��2q����� *LicensePath+�  �� $	��100 ��� $	��200��.+��� $��0��  	_dataPath#��[ �� $��1��$��2q����� *
DataPath+�� $ Vertical=�$��1��.+���   
_textInput#��[ �� $	��200�� �� AcceptsReturn .  �� TextWrapping$
 Wrap=����� *	TextInput+�� $ Vertical=�$��2��.+��  �� $
��Auto ��� $
��Auto � �� $��* ��� $��3* ��� $��3* ��� $��2* �.+��� $��0��$��0�� 
 _score#��[ �� $��1��$��0��  _phrases#��[ �� $��2��$��0��  _modelSentiment#��[ �� $��3��$��0��  	_emotions#��[ �� $	��100��$��0��$��1��$	��0.5q�
���   �._��� ���� *Score+�� $��1��$��1��$	��0.5q�
���   �._��� ���� *Phrase+�� $��2��$��1��$	��0.5q�
���   �._��� ���� *ModelSentiment+�� $��3��$��1��$	��0.5q�
���   �._��� �� $��3��$
��0 10q�$ 
Horizontal=�$��Right=�$	��Top=�.+��� $��3q�$��10q�  _update#��[ �� $��3q�$��10q� 
	 _reset#��[	 ���� *ResetCommand+