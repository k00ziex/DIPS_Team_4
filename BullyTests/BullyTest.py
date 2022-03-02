import unittest
from OriginalBullyProcess import OriginalBullyProcess


class Basic_Election(unittest.TestCase):
    def test_4IsElected(self):
        self.node1 = OriginalBullyProcess(1)
        self.node2 = OriginalBullyProcess(2)
        self.node3 = OriginalBullyProcess(3)
        self.node4 = OriginalBullyProcess(4)

        nodeArray = [self.node1, self.node2, self.node3, self.node4]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        self.node1.startElection()

        self.assertEqual(True, self.node4.isLeader())  # add assertion here
        self.assertEqual(False, self.node3.isLeader())  # add assertion here
        self.assertEqual(False, self.node2.isLeader())  # add assertion here
        self.assertEqual(False, self.node1.isLeader())  # add assertion here

class Singular_Node_Crash_Election(unittest.TestCase):
    def test_node2Killed_1isElected(self):
        self.node1 = OriginalBullyProcess(1)
        self.node2 = OriginalBullyProcess(2)

        self.nodeArray = [self.node1, self.node2]

        for node in self.nodeArray:
            node.setOtherProcessIDs(self.nodeArray)

        self.node2.killNode()
        self.node1.startElection()

        #print(node2.isLeader())
        #print(node1.isLeader())

        self.assertEqual(False, self.node2.isLeader())  # add assertion here
        self.assertEqual(True, self.node1.isLeader())  # add assertion here
        

class Killed_Node_Reboots_Election(unittest.TestCase):
    def test_node3KilledAndStarted_3isElected(self):
        
        self.node1 = OriginalBullyProcess(1)
        self.node2 = OriginalBullyProcess(2)
        self.node3 = OriginalBullyProcess(3)

        nodeArray = [self.node1, self.node2, self.node3]

        for node in nodeArray:
            node.setOtherProcessIDs(nodeArray)

        self.node3.killNode()
        self.node1.startElection()

        print(self.node2.whoseLeader)
        print(self.node1.whoseLeader)

        self.node3.startNode()

        print(self.node2.whoseLeader)
        print(self.node1.whoseLeader)
        
        self.assertEqual(True, self.node3.isLeader())  # add assertion here
        self.assertEqual(False, self.node2.isLeader())  # add assertion here
        self.assertEqual(False, self.node1.isLeader())  # add assertion here



if __name__ == '__main__':
    unittest.main()
