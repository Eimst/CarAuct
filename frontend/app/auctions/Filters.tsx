
import React from 'react';
import {Button, ButtonGroup} from "flowbite-react";
import {useParamStore} from "@/hooks/useParamsStore";
import {AiOutlineClockCircle, AiOutlineSortAscending} from "react-icons/ai";
import {BsFillStopCircleFill, BsStopwatchFill} from "react-icons/bs";
import {GiFinishLine, GiFlame} from "react-icons/gi";


const pageSizeButtons = [4, 8, 12]

const orderButtons = [
    {
        label: 'Alphabetical',
        icon: AiOutlineSortAscending,
        value: 'make'
    },
    {
        label: 'End date',
        icon: AiOutlineClockCircle,
        value: 'endingSoon'
    },
    {
        label: 'Recently added',
        icon: BsFillStopCircleFill,
        value: 'new'
    }
]

const filterButtons = [
    {
        label: 'Live',
        icon: GiFlame,
        value: 'live'
    },
    {
        label: 'Ending < 6 hours',
        icon: GiFinishLine,
        value: 'endingSoon'
    },
    {
        label: 'Completed',
        icon: BsStopwatchFill,
        value: 'finished'
    }
]

const Filters = () => {
    const pageSize = useParamStore(state => state.pageSize)
    const setParams = useParamStore(state => state.setParams)

    const orderBy = useParamStore(state => state.orderBy)
    const filterBy = useParamStore(state => state.filterBy)

    return (
        <div className="flex justify-between items-center mb-6">
            <div>
                <span className='uppercase text-sm text-gray-500 mr-2'>Filter by</span>
                <ButtonGroup>
                    {
                        filterButtons.map(({label, icon: Icon, value}) => (
                            <Button
                                key={value}
                                onClick={() => setParams({filterBy: value})}
                                color={`${filterBy === value ? 'red' : 'gray'}`}
                                className={`focus:ring-0 border`}
                            >
                                <Icon className='mr-3 h-4 w-4 mt-0.5'/>
                                {label}
                            </Button>
                        ))
                    }
                </ButtonGroup>
            </div>

            <div>
                <span className='uppercase text-sm text-gray-500 mr-2'>Order by</span>
                <ButtonGroup>
                    {
                        orderButtons.map(({label, icon: Icon, value}) => (
                            <Button
                                key={value}
                                onClick={() => setParams({orderBy: value})}
                                color={`${orderBy === value ? 'red' : 'gray'}`}
                                className={`focus:ring-0 border`}
                            >
                                <Icon className='mr-3 h-4 w-4 mt-0.5'/>
                                {label}
                            </Button>
                        ))
                    }
                </ButtonGroup>
            </div>

            <div>
                <span className='uppercase text-sm text-gray-500 mr-2'>Page size</span>
                <ButtonGroup>
                    {pageSizeButtons.map((value, i) => (
                        <Button key={i} onClick={() => setParams({pageSize: value})}
                                color={`${pageSize === value ? 'red' : 'gray'}`}
                                className={`focus:ring-0 border `}
                        >
                            {value}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
        </div>
    );
};

export default Filters;