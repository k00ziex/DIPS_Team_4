import unittest
import random
from ImprovedBullyProcess import ImprovedBullyProcess
from OriginalBullyProcess import OriginalBullyProcess

class ComparisonCases(unittest.TestCase):
    def test_Original_10AliveNodes(self):
        listOfP = []
        networkMessagesSent = [0]

        for i in range(1,10):
            process = OriginalBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
                
        for p in listOfP:
            p.setOtherProcessIDs(listOfP)

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
        
        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

    def test_improved_10_nodes_none_dead(self):
        listOfP = []
        networkMessagesSent = [0]

        for i in range(1,10):
            process = ImprovedBullyProcess(i, networkMessagesSent)
            listOfP.append(process)

        for p in listOfP:
            p.setNeighbors(listOfP)

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
        
        print(f"Messages Sent in Network: {networkMessagesSent[0]}")


    def test_Original_10AliveNodes_ShuffledListOfProcesses(self):
        listOfP = []
        networkMessagesSent = [0]

        for i in range(1,10):
            process = OriginalBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
        random.Random(42).shuffle(listOfP) # We seed Random with 42 to get the same randomness every time

        for p in listOfP:
            p.setOtherProcessIDs(listOfP)

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
        
        print(f"Messages Sent in Network: {networkMessagesSent[0]}")