import unittest
from ImprovedBullyProcess import ImprovedBullyProcess

class MyTestCase(unittest.TestCase):
    def test_something(self):

        ListOfP = []

        p1 = ImprovedBullyProcess(10)
        ListOfP.append(p1)
        p2 = ImprovedBullyProcess(9)
        ListOfP.append(p2)
        p3 = ImprovedBullyProcess(11)
        ListOfP.append(p3)
        p4 = ImprovedBullyProcess(2)
        ListOfP.append(p4)

        p1.setNeighbors(ListOfP)
        p2.setNeighbors(ListOfP)
        p3.setNeighbors(ListOfP)
        p4.setNeighbors(ListOfP)

        p1.startElection()

        print(p1.whoseLeader)
        print(p2.whoseLeader)
        print(p3.whoseLeader)
        print(p4.whoseLeader)





if __name__ == '__main__':
    unittest.main()
