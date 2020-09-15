#Гомоку

##Особенности программы:
1. Возможна игра компьютер против компьютера и человек против компьютера. 
2. Возможны настраиваемые параметры стратегии игры компьютера, когда он играет против человека, и каждого из игроков при игре компьютер против компьютера.
3. При игре человек-компьютер можно выбрать, кто будет ходить первым.
4. Возможно задание произвольного размера поля.


Интерфейс - консоль, есть меню для выбора некоторых параметров перед запуском, ввод координат хода как в шахматах.

Описание организации игры:
- из метода Main класса Gomoku запукаются методы, находящиеся в том же классе, для игры компьютер с компьютером (void CompVsComp()) или человек с компьютером (void CompVsHuman()).
- в каждом из этих методов в цикле do - while делается поиск следующего хода или запрос хода у человека. Цикл прекращается при выигрыше любого из игроков или при заполнении доски ("ничья"").
- ключевой класс игры - игровая доска class Board, в котором определен массив char[,] BoardArray ячеек игрового поля (zero-based), а также методы для поиска хода.
- для поиска хода используется метод Coordinates FindNextMove(int PlayerNo), принимающий номер игрока (one-based) и возвращающий объект координат следующего хода.
- Метод FindNextMove в свою очередь использует метод void MakeAnalysisMap(int PlayerNo, bool Attack, float[,] analysisMap), возвращающий карту (еще один массив игровой доски), заполненную рейтингами следующего удачного хода. Подробнее об алгоритме ниже. Массив возвращается по ссылке, т.е. происходит модификация входного массива.
- при поиске хода алгоритм учитывает параметры стратегии, определенные в объекте класса NextMoveParameters, доступ к которому выполняется через индексатор для выбора пресета стратегических параметров. Объект заполняется пресетами параметров при своем создании в конструкторе.

Описание алгоритма:
Для поиска следующего хода делается анализ каждой ячейки игрового поля и составляется карта рейтингов успешности хода. Затем в готовой карте ищется ячейка с максимальным рейтингом и координаты этой ячейки отправляются вызывающему методу. Если есть ячейки с одинаковыми максимальными рейтингами, из них делается случайная выборка.
Карта рейтингов успешности состоит из двух частей: карта при атаке и карта при обороне. Карта при обороне составляется абсолютно так же, как карта при атаке, но для противоположного игрока. Итоговая карта является суммой двух карт. Таким образом, алгоритм в этой части "думает за противника", закрывая противнику ходы, которые принесли бы ему максимальную выгоду.

Как считаются рейтинги?
При условии, что ячейка пустая, начинается подсчет длины цепочек камней, частью которых может стать ячейка. Подсчет делается в 4 этапа: по горизонтали, по вертикали, в двух диагональных направлениях. Каждое направление исследуется в две стороны на 4 шага от ячейки (счетчики countForward и countBackward). Затем все направления складываются в итоговом рейтинге factor.
При подсчете итогового рейтинга factor в простейшем случае (без применения стратегических параметров) суммируются длины цепочек. Например, если ячейка может стать частью цепочки длиной 3 камня, то ее рейтинг равен 2 (считаются уже стоящие камни). Если возможна цепочка 5 ячеек, то рейтинг будет 4. Если ячейка может стать частью цепочек в нескольких направлениях, то все они суммируются. Например, если ячейка может стать частью цепочки в 2 камня, и одновременно в другом направлении частью цепочки в 4 камня, то итоговый рейтинг будет 1+3=4.

О параметрах стратегии.
Очевидно, что рейтинг ячейки, которая станет частью цепочки в 5 ячеек (победа) должен быть гораздо выше, чем сумма любых других направлений и цепочек. Так, в примере выше цепочки в 2 и 4 камня дадут итоговый рейтинг в 4, столько же, сколько и у выигрышной цепочки в 5 камней, но не приведут к победе, поэтому надо как-то выделить большим рейтингом цепочку длиной в 5 камней, чтобы был сделан автоматический выбор этого выигрышного хода. Для придания дополнительного веса ячейке, счетчики которой по одной линии дают 4 (один шаг до победы), используется множитель float ImportanceOf4. В каждой из двух стратегий он равен 10F. 
По аналогии можно придать разные веса цепочкам из 3 камней, 2 камней и 1 камня, чтобы в итоговой сумме рейтингов factor учитывались эти различия в значимости цепочек. Так, цепочка в 4 камня может быть важнее цепочки в 3 камня (спорный вопрос).
Дополнительно настраивается значимость открытой цепочки. Есть дополнительная проверка того, что в каком-то направлении будет открытая с обоих концов цепочка, что делает ее гораздо перспективнее. Для этого используется флаг bool EmptyChain и стратегический параметр float ImportanceOfEmptyChain.
Дополнительно настраивается значимость рейтингов обороны и рейтингов нападения (ImportanceOfDefence и ImportanceOfAttack) для того, чтобы в суммированной карте было больше влияния оборонной логики (закрываем противнику его ходы) или нападающей логики (пытаемся побыстрее выстраивать свои цепочки).
К примеру, в первом пресете стратегических параметров используются такие значения как множители в суммарном рейтинге:
par[0].ImportanceOf1 = 1F;
par[0].ImportanceOf2 = 1.1F;
par[0].ImportanceOf3 = 1.3F;
par[0].ImportanceOf4 = 10F;
par[0].ImportanceOfAttack = 1F;
par[0].ImportanceOfDefence = 1F;
par[0].ImportanceOfEmptyChain = 1.3F;

Пресетов может быть сколько угодно. Можно устраивать состязания двух игроков компьютер-компьютер, которые используют разные стратегии.

Размер игрового поля можно менять в переменной BoardSize класса Gomoku.

Как можно улучшить программу: распознавание введенных с клавиатуры символов не содержит проверки ошибок ввода и исключений; в меню можно выбрать только 2 пресета для параметров; нет журнала ходов игры.

P.S. Для наглядного представления о том, как работают рейтинги, можно выводить на экран текущие карту рейтингов обороны и карту рейтингов нападения, если раскомментить строки 437-438:
// for testing
//ShowAnalysisMap(mapOfAttack);
//ShowAnalysisMap(mapOfDefence);

При этом для лучшего размещения надписей на экране рекомендую в строках 77 и 136 изменить параметр "+ 5" на "+ 36":
textRow = boardRow + BoardSize + 5;

В тексте программы есть комментарии, которые, надеюсь, могут помочь сориентироваться.

Антон Ермолаев (anton.yermolayev@outlook.com, +7 777 268 20 11), 03.09.2020
